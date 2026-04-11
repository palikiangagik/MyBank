using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyBank.Portal.Areas.Portal.ViewModels;
using MyBank.Portal.Data;
using SQLitePCL;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using MyBank.Portal.Models;

namespace MyBank.Portal.Areas.Portal.Controllers
{
    public class MoneyTransferController : BaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly MyBankPortalContext _context;

        public MoneyTransferController(UserManager<IdentityUser> userManager,
            MyBankPortalContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (null == user)
                return Problem("User not found"); // Move to a common place, e.g. BaseController

            return View(await GetViewModel(user));
        }

        [HttpPost]
        public async Task<IActionResult> Index(MoneyTransferViewModel viewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (null == user)
                return Problem("User not found"); // Move to a common place, e.g. BaseController
            
            if (!ModelState.IsValid)
                return View(await GetViewModel(user));
            
            try
            {
                var accountFrom = await _context.Accounts
                    .Where(acc => acc.Id == viewModel.FromAccount && acc.User.Id == user.Id && !acc.IsClosed)
                    .FirstOrDefaultAsync();

                if (null == accountFrom) 
                    throw new ValidationException("From account not found");

                var accountTo = await _context.Accounts
                    .Where(acc => acc.Id == viewModel.ToAccount && !acc.IsClosed)
                    .FirstOrDefaultAsync();

                if (null == accountTo)
                    throw new ValidationException("To account not found");

                if (accountTo == accountFrom)
                    throw new ValidationException("From and To accounts must be different");

                if (viewModel.Amount > accountFrom.Balance)
                    throw new ValidationException("Not enough balance");

                accountFrom.Balance -= viewModel.Amount;
                accountTo.Balance += viewModel.Amount;

                _context.Transactions.Add(new Transaction
                {
                    Type = TransactionType.Transfer,
                    Amount = viewModel.Amount,
                    Sender = accountFrom,
                    Recipient = accountTo
                });


                await _context.SaveChangesAsync();

                TempData["Message"] = $"{viewModel.Amount} sent successfully";
            }
            catch (ValidationException e)
            {
                ModelState.AddModelError("", e.Message);
                return View(await GetViewModel(user));
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError("", "An error occurred while processing your request. Please try again.");
                return View(await GetViewModel(user));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unexpected error occurred. Please try again.");
                return View(await GetViewModel(user));
            }

            return RedirectToAction("Index");
        }


        private async Task<MoneyTransferViewModel> GetViewModel(IdentityUser user)
        {
            var accountsFrom = await _context.Accounts.Where(acc => acc.User.Id == user.Id && !acc.IsClosed)
                .Select(acc => new SelectListItem
                {
                    Value = acc.Id.ToString(),
                    Text = $"{acc.Id} (Balance: {acc.Balance})" // TODO: move to a helper method
                })
                .ToListAsync();
            
            // TODO: Result could be too big.
            var accountsTo = await _context.Accounts
                .Where(acc => !acc.IsClosed)
                .Select(acc => new SelectListItem
                {
                    Value = acc.Id.ToString(),
                    Text = $"{acc.User.UserName} - {acc.Id}" // TODO: move to a helper method
                })
                .ToListAsync();

            return new MoneyTransferViewModel
            {
                FromAccounts = accountsFrom,
                ToAccounts = accountsTo
            };
        }
    }
}
