using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBank.Application.UseCases;
using MyBank.Api.DTO.Common;
using MyBank.Api.DTO.Accounts;
using MyBank.Api.DTO.Transactions;
using MyBank.Api.Mappings;


namespace MyBank.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class AccountsController : BaseController
    {
        private readonly AccountsUseCases _accountsUseCases;

        public AccountsController(AccountsUseCases accountsUseCases)
        {
            _accountsUseCases = accountsUseCases;
        }

        [HttpGet]
        public async Task<IActionResult> GetAccounts([FromQuery] PagingParametersDTO dto)
        {
            var result = await _accountsUseCases.GetClientAccountListAsync(ClientId, dto.ToAppDTO());
            return Ok(result.ToApiDTO());
        }

        [HttpGet("destinations")]
        public async Task<IActionResult> GetDestinationAccounts([FromQuery] PagingParametersDTO dto)
        {
            var result = await _accountsUseCases.GetDestinationAccountListAsync(ClientId, dto.ToAppDTO());
            return Ok(result.ToApiDTO());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccount(int id)
        {
            var result = await _accountsUseCases.GetAccountSummaryAsync(ClientId, id);
            return result.Failed ? Failure(result) : Ok(result.Value.ToApiDTO());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CloseAccount(int id)
        {
            var result = await _accountsUseCases.CloseAccountAsync(ClientId, id);
            return result.Failed ? Failure(result) : NoContent();
        }

        // open new account
        [HttpPost]
        public async Task<IActionResult> OpenAccount(OpenAccountRequestDTO dto)
        {
            var result = await _accountsUseCases.OpenAccountAsync(ClientId, dto.Amount);
            if (result.Failed)
                return Failure(result);
            return CreatedAtAction(nameof(GetAccount), new { id = result.Value.Id }, result.Value.ToApiDTO());
        }

        [HttpPost("{id}/deposit")]
        public async Task<IActionResult> Deposit(int id, DepositRequestDTO dto)
        {
            var result = await _accountsUseCases.DepositAsync(ClientId, id, dto.Amount);
            return result.Failed ? Failure(result) : Ok(result.Value.ToApiDTO());
        }

        [HttpPost("{id}/withdraw")]
        public async Task<IActionResult> Withdraw(int id, WithdrawalRequestDTO dto)
        {
            var result = await _accountsUseCases.WithdrawAsync(ClientId, id, dto.Amount);
            return result.Failed ? Failure(result) : Ok(result.Value.ToApiDTO());
        }

        [HttpPost("{id}/transfer")]
        public async Task<IActionResult> Transfer(int id, TransferRequestDTO dto)
        {
            var result = await _accountsUseCases.TransferAsync(ClientId, id, dto.RecipientAccountId, dto.Amount);
            return result.Failed ? Failure(result) : Ok(result.Value.ToApiDTO());
        }
    }
}
