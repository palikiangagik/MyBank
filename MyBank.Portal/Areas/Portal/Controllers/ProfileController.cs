using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyBank.Portal.Areas.Portal.ViewModels;
using MyBank.Portal.Data;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MyBank.Portal.Models;

namespace MyBank.Portal.Areas.Portal.Controllers
{
    public class ProfileController : BaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly MyBankPortalContext _context;

        public ProfileController(UserManager<IdentityUser> userManager, 
            MyBankPortalContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Problem("User not found");

            var accounts = await _context.Accounts
                .Where(acc => acc.User == user && !acc.IsClosed)
                .Select(acc => new ProfileAccountViewItem { 
                    Id = acc.Id, 
                    Balance = acc.Balance
                })
                .ToListAsync();

            decimal totalBalance = accounts.Sum(acc => acc.Balance);

            return View(new ProfileViewModel {
                UserName = user.UserName,
                Balance = totalBalance,
                Accounts = accounts,
            });
        }

        public async Task<IActionResult> OpenNewAccount()
        {
            return View(new OpenNewAccountViewModel());            
        }

        [HttpPost]
        public async Task<IActionResult> OpenNewAccount(OpenNewAccountViewModel vm)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Problem("User not found");

            if (!ModelState.IsValid)
                return View(new OpenNewAccountViewModel());

            await _context.Accounts.AddAsync(new Account
            {
                User = user,
                Balance = vm.Amount
            });

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> CloseAccount(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Problem("User not found");

            bool exist = await _context.Accounts.AnyAsync(a => a.Id == id && a.User == user && !a.IsClosed);

            if (!exist)
                return NotFound();

            return View(new CloseAccountViewModel { Id = id });
        }


        [HttpPost]
        public async Task<IActionResult> CloseAccount(CloseAccountViewModel vm)
        {   
            var user = await _userManager.GetUserAsync(User);
            
            if (user == null)
                return NotFound();

            var account = await _context.Accounts
                .Where(a => a.Id == vm.Id && a.User == user && !a.IsClosed)
                .FirstOrDefaultAsync();

            if (account == null)
                return NotFound();

            account.IsClosed = true;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
