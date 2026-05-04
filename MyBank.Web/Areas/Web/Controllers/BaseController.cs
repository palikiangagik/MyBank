using CorePrimitives;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MyBank.Infrastructure.Identity;
using MyBank.Web.Areas.Web.ViewModels;
using System;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyBank.Web.Areas.Web.Controllers
{
    [Authorize]
    [Area("Web")]
    public abstract class BaseController : Controller
    {
        public int ClientId
        {
            get
            {
                var value = User.FindFirst("ClientId")?.Value;

                int clientId = 0;
                if (!int.TryParse(value, out clientId))
                    throw new SecurityException("Client not found or can't be parsed.");

                return clientId;
            }
        }

        /// <summary>
        /// Return the appropriate failure response based on the error type. 
        /// If the error type is not recognized, it returns a generic 500 error with the error description.
        /// If model is specified and error type is validation, the error is returned by the model (ModelState).
        /// </summary>
        /// <param name="result"> The result object containing error information.</param>
        /// <param name="model">(Optional) The model to return to the view in case of validation errors.</param>
        /// <param name="action">(Optional) The name of the action to return to in case of validation errors. 
        /// If null - current action is used.</param>
        /// <returns>An IActionResult representing the outcome of the operation.</returns>
        protected IActionResult Failure(Result result, BaseViewModel viewModel = null, string action = null)
        {
            if (result.Error is null)
                return StatusCode(500, "An unknown error occurred.");

            if (viewModel is null)
            {
                //string error = string.Join("\n", result.Errors.Select(e => e.Description));                
                //return BadRequest(error);
                switch (result.Error.Type)
                {
                    case ErrorType.NotFound:
                        return NotFound(result.Error.Description); // 404
                    case ErrorType.Conflict:
                        return Conflict(result.Error.Description); // 409 
                    case ErrorType.Unauthorized:
                        return Unauthorized(result.Error.Description); // 401
                }
            }

            ModelState.AddModelError(string.Empty, result.Error.Description);//foreach (var error in result.Errors)
            //    ModelState.AddModelError(error.Id, error.Description);

            if (string.IsNullOrEmpty(action))
                return View(viewModel);
            
            return View(action, viewModel);
        }
    }
}
