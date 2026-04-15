using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBank.Portal.Infrastructure;
using System.Security.Claims;

namespace MyBank.Portal.Areas.Portal.Controllers
{
    [Authorize]
    [Area("Portal")]
    public abstract class BaseController : Controller
    {
        protected string UserName => User.FindFirstValue(ClaimTypes.Name);
        protected string UserNameIdentifier => User.FindFirstValue(ClaimTypes.NameIdentifier);

        /// <summary>
        /// Return the appropriate failure response based on the error type. 
        /// If the error type is not recognized, it returns a generic 500 error with the error description.
        /// If model is specified and error type is validation, the error is returned by the model (ModelState).
        /// </summary>
        /// <param name="result">The result object containing error information.</param>
        /// <param name="model">(Optional) The model to return to the view in case of validation errors.</param>
        /// <param name="action">(Optional) The name of the action to return to in case of validation errors. 
        /// If null - current action is used.</param>
        /// <returns>An IActionResult representing the outcome of the operation.</returns>
        protected IActionResult Failure(Result result, object model = null, string action = null)
        {
            if (result.Error == null)
                return StatusCode(500, "An unknown error occurred.");

            if (null != model && result.Error.Type == ErrorType.Validation)
            {
                ModelState.AddModelError(string.Empty, result.Error.Description);
                if (string.IsNullOrEmpty(action))
                    return View(model);
                else
                    return View(action, model);
            }

            return result.Error.Type switch
            {
                ErrorType.NotFound => NotFound(result.Error.Description),
                ErrorType.Validation => BadRequest(result.Error.Description),
                _ => StatusCode(500, result.Error.Description)
            };
        }
    }
}
