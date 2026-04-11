using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyBank.Portal.Data;

namespace MyBank.Portal.Areas.Portal.Controllers
{

    [Authorize]
    [Area("Portal")]
    public abstract class BaseController : Controller
    {    
    }
}
