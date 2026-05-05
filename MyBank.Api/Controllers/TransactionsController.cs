using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBank.Application.UseCases;
using MyBank.Api.DTO.Common;
using MyBank.Api.Mappings;


namespace MyBank.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class TransactionsController : BaseController
    {
        private readonly TransactionsUseCases _transactionsUseCases;

        public TransactionsController(TransactionsUseCases transactionsUseCases)
        {
            _transactionsUseCases = transactionsUseCases;
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactions([FromQuery] PagingParametersDTO dto)
        {
            var result = await _transactionsUseCases.GetTransactionHistoryAsync(ClientId, dto.ToAppDTO());
            return Ok(result.ToApiDTO());                
        }
    }
}