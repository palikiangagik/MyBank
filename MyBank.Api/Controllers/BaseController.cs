using CorePrimitives;
using Microsoft.AspNetCore.Mvc;
using System.Security;

namespace MyBank.Api.Controllers
{
    [ApiController]
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
        /// </summary>
        /// <param name="result"> The result object containing error information.</param>
        /// <returns>An IActionResult representing the outcome of the operation.</returns>
        protected IActionResult Failure(Result result)
        {
            if (result.Error is null)
                return StatusCode(500, "An unknown error occurred.");            

            switch (result.Error.Type)
            {
                case ErrorType.NotFound:
                    return NotFound(result.Error.Description); // 404
                case ErrorType.Conflict:
                    return Conflict(result.Error.Description); // 409 
                case ErrorType.Unauthorized:
                    return Unauthorized(result.Error.Description); // 401
                default: 
                    return StatusCode(500, result.Error.Description); // 500 for unrecognized error types
            }            
        }
    }
}
