using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyBank.Portal.Areas.Portal.ViewModels;
using MyBank.Portal.Data;
using System.Threading.Tasks;
using System.Linq;

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
                return NotFound();

            var accounts = from acc in _context.Accounts
                           where acc.User == user
                           select acc;

            decimal balance = 0;
            foreach (var acc in accounts)
            {
                balance += acc.Balance;
            }

            return View(new ProfileViewModel {
                UserName = user.UserName,
                Balance = balance
            });
        }
    }
}
