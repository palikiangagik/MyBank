using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyBank.Portal.Areas.Portal.Controllers
{
    [Authorize]
    [Area("Portal")]
    public class FundsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
