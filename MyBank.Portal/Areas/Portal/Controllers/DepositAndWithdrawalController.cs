using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyBank.Portal.Areas.Portal.ViewModels;
using MyBank.Portal.Data;
using MyBank.Portal.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyBank.Portal.Areas.Portal.Controllers
{
    public class DepositAndWithdrawalController : BaseController
    {

        private readonly MyBankPortalContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DepositAndWithdrawalController(MyBankPortalContext context,
            UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (null == user)
                return Problem("User not found");

            return View(await GetViewModel(user));
        }

        [HttpPost]
        public async Task<IActionResult> Index(DepositAndWithdrawalViewModel viewModel, string actionType)
        {
            // TODO: +move the code to the service
            // TODO: +add logging, +add IStringLocalizer

            var user = await _userManager.GetUserAsync(User);

            if (null == user)
                return Problem("User not found"); // Move to a common place, e.g. BaseController


            if (!ModelState.IsValid)
                return View(await GetViewModel(user));

            try
            {    
                Account dbacc = await (from acc in _context.Accounts
                                       where acc.Id == viewModel.Account && acc.User == user && !acc.IsClosed
                                       select acc).FirstOrDefaultAsync();
                if (null == dbacc)
                    throw new ValidationException("Account not found");
                
                decimal amount = actionType == "Withdrawal" ? -viewModel.Amount : viewModel.Amount;

                if (dbacc.Balance + amount < 0)
                    throw new ValidationException("Not enough balance");                


                await _context.Transactions.AddAsync(new Transaction
                {
                    Type = actionType == "Withdrawal" ? TransactionType.Withdrawal : TransactionType.Deposit,
                    Amount = Math.Abs(amount),
                    Sender = actionType == "Withdrawal" ? dbacc : null,
                    Recipient = actionType == "Deposit" ? dbacc : null
                });

                dbacc.Balance += amount;

                await _context.SaveChangesAsync();

                TempData["Message"] = amount < 0 ?
                    $"Withdrew {-amount} from account {dbacc.Id} successfully" :
                    $"Deposited {amount} to account {dbacc.Id} successfully";
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


            return RedirectToAction("Index"); // TODO: use nameof()
        }

        private async Task<DepositAndWithdrawalViewModel> GetViewModel(IdentityUser user)
        {
            var accounts = await (from acc in _context.Accounts
                                  where acc.User == user && !acc.IsClosed
                                  select new SelectListItem
                                  {
                                      Value = acc.Id.ToString(),
                                      Text = $"{acc.Id} (Balance: {acc.Balance})"
                                  }).ToListAsync();

            return new DepositAndWithdrawalViewModel { Accounts = accounts };
        }
    }
}
