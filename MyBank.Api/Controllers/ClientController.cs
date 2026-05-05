using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBank.Application.UseCases;
using MyBank.Api.DTO.Common;
using MyBank.Api.Mappings;


namespace MyBank.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ClientController : BaseController
    {
        private readonly ClientUseCases _clientUseCases;

        public ClientController(ClientUseCases clientUseCases)
        {
            _clientUseCases = clientUseCases;
        }

        // get client summary
        [HttpGet]
        public async Task<IActionResult> GetClientSummary([FromQuery] PagingParametersDTO dto)
        {
            var result = await _clientUseCases.GetClientSummaryAsync(ClientId, dto.ToAppDTO());
            return result.Failed ? Failure(result) : Ok(result.Value.ToApiDTO());
        }
    }
}
