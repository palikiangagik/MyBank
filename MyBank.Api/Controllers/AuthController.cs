using Microsoft.AspNetCore.Mvc;
using MyBank.Infrastructure.Identity;

using ApiDTO = MyBank.Api.DTO;
using InfrDTO = MyBank.Infrastructure.DTO;

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
        public async Task<IActionResult> Register(ApiDTO.RegisterRequestDTO dto)
        {
            var result = await _clientIdentityService.RegisterClientAsync(new InfrDTO.RegisterClientDTO
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = dto.Password,
            });

            if (result.Failed)
                return Failure(result);
            return NoContent();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(ApiDTO.LoginRequestDTO dto)
        {
            var result = await _clientIdentityService.LoginClientAsync(new InfrDTO.LoginClientDTO { 
                Email = dto.Email,
                Password = dto.Password,
                RememberMe = dto.RememberMe
            });

            if (result.Failed)
                return Failure(result);
            return NoContent();
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _clientIdentityService.LogoutAsync();
            return NoContent();
        }
    }
}
