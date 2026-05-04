using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBank.Application.UseCases;

using ApiDTO = MyBank.Api.DTO;
using AppDTO = MyBank.Application.DTO;

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
        public async Task<IActionResult> GetClientSummary([FromQuery] ApiDTO.PagingParametersDTO dto)
        {
            var result = await _clientUseCases.GetClientSummaryAsync(ClientId, new(dto.Page, dto.PageSize));
            if (result.Failed)
                return Failure(result);
            AppDTO.ClientSummaryDTO summary = result.Value;

            return Ok(new ApiDTO.ClientSummaryDTO {
                ClientName = summary.Name.FirstName + " " + summary.Name.LastName,
                Balance = summary.TotalBalance,
                Accounts = summary.AccountList.Items
                .Select(acc => new ApiDTO.ClientSummaryAccountItemDTO
                {
                    Id = acc.Id,
                    Code = acc.Code,
                    Balance = acc.Balance
                }).ToList()
            });
        }
    }
}
