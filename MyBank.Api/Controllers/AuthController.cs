using Microsoft.AspNetCore.Mvc;
using MyBank.Infrastructure.Identity;
using MyBank.Api.DTO.Auth;
using MyBank.Api.Mappings;


namespace MyBank.Api.Controllers
{    
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly ClientIdentityService _clientIdentityService;

        public AuthController(ClientIdentityService clientIdentityService)
        {
            _clientIdentityService = clientIdentityService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDTO dto)
        {
            var result = await _clientIdentityService.RegisterClientAsync(dto.ToInfraDTO());
            return result.Failed ? Failure(result) : NoContent();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDTO dto)
        {
            var result = await _clientIdentityService.LoginClientAsync(dto.ToInfraDTO());
            return result.Failed ? Failure(result) : NoContent();
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _clientIdentityService.LogoutAsync();
            return NoContent();
        }
    }
}
