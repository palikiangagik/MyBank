using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyBank.Portal.Areas.Portal.ViewModels;
using MyBank.Portal.Data;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MyBank.Portal.Areas.Portal.Controllers
{
    [Authorize]
    [Area("Portal")]
    public class ProfileController : Controller
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
                .Where(acc => acc.User == user)
                .Select(acc => new ProfileAccountViewItem { Id = acc.Id, Balance = acc.Balance })
                .ToListAsync();

            decimal totalBalance = accounts.Sum(acc => acc.Balance);

            return View(new ProfileViewModel {
                UserName = user.UserName,
                Balance = totalBalance,
                Accounts = accounts
            });
        }
    }
}
