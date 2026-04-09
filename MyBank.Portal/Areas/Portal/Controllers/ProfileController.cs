using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyBank.Portal.Areas.Portal.ViewModels;

namespace MyBank.Portal.Areas.Portal.Controllers
{
    [Authorize]
    [Area("Portal")]
    public class ProfileController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ProfileController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            ProfileViewModel vm = new ProfileViewModel();
            vm.UserName = _userManager.GetUserName(User);
            return View(vm);
        }
    }
}
