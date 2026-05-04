using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBank.Application.UseCases;

using ApiDTO = MyBank.Api.DTO;
using AppDTO = MyBank.Application.DTO;

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
        public async Task<IActionResult> GetAccounts([FromQuery] ApiDTO.PagingParametersDTO dto)
        {
            var result = await _accountsUseCases.GetClientAccountListAsync(ClientId, new(dto.Page, dto.PageSize));
            return Ok(new ApiDTO.ClientAccountsDTO
            {
                TotalCount = result.TotalCount,
                Accounts = result.Items.Select(a => new ApiDTO.AccountSummaryDTO
                {
                    Id = a.Id,
                    Code = a.Code,
                    Balance = a.Balance
                }).ToList()
            });
        }

        [HttpGet("destinations")]
        public async Task<IActionResult> GetDestinationAccounts([FromQuery] ApiDTO.PagingParametersDTO dto)
        {
            var result = await _accountsUseCases.GetDestinationAccountListAsync(ClientId, new(dto.Page, dto.PageSize));
            return Ok(new ApiDTO.ClientDestinationAccountsDTO
            {
                TotalCount = result.TotalCount,
                Accounts = result.Items.Select(a => new ApiDTO.DestinationAccountDTO
                {
                    Id = a.Id,
                    Code = a.Code,
                    Name = new ApiDTO.DestinationAccountDTO.ClientName
                    {
                        FirstName = a.Name.FirstName,
                        LastName = a.Name.LastName
                    }
                }).ToList()
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccount(int id)
        {
            var result = await _accountsUseCases.GetAccountSummaryAsync(ClientId, id);
            if (result.Failed)
                return Failure(result);
            AppDTO.AccountSummaryDTO summary = result.Value;
            return Ok(new ApiDTO.AccountSummaryDTO
            {
                Id = summary.Id,
                Code = summary.Code,
                Balance = summary.Balance
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CloseAccount(int id)
        {
            var result = await _accountsUseCases.CloseAccountAsync(ClientId, id);
            if (result.Failed)
                return Failure(result);
            return NoContent();
        }

        // open new account
        [HttpPost]
        public async Task<IActionResult> OpenAccount(ApiDTO.OpenAccountRequestDTO dto)
        {
            var result = await _accountsUseCases.OpenAccountAsync(ClientId, dto.Amount);
            if (result.Failed)
                return Failure(result);
            AppDTO.AccountSummaryDTO summary = result.Value;
            return CreatedAtAction(
                nameof(GetAccount),
                new { id = summary.Id },
                new ApiDTO.AccountSummaryDTO
                {
                    Id = summary.Id,
                    Code = summary.Code,
                    Balance = summary.Balance
                }
            );
        }

        [HttpPost("{id}/deposit")]
        public async Task<IActionResult> Deposit(int id, ApiDTO.DepositRequestDTO dto)
        {
            var result = await _accountsUseCases.DepositAsync(ClientId, id, dto.Amount);
            if (result.Failed)
                return Failure(result);
            AppDTO.DepositTransactionDTO trans = result.Value;

            return Ok(new ApiDTO.DepositTransactionDTO {
                CreatedAt = trans.CreatedAt,
                Amount = trans.Amount,
                AccountCode = trans.AccountCode
            });
        }

        [HttpPost("{id}/withdraw")]
        public async Task<IActionResult> Withdraw(int id, ApiDTO.WithdrawalRequestDTO dto)
        {
            var result = await _accountsUseCases.WithdrawAsync(ClientId, id, dto.Amount);
            if (result.Failed)
                return Failure(result);
            AppDTO.WithdrawalTransactionDTO trans = result.Value;

            return Ok(new ApiDTO.WithdrawalTransactionDTO
            {
                CreatedAt = trans.CreatedAt,
                Amount = trans.Amount,
                AccountCode = trans.AccountCode
            });
        }


        [HttpPost("{id}/transfer")]
        public async Task<IActionResult> Transfer(int id, ApiDTO.TransferRequestDTO dto)
        {
            var result = await _accountsUseCases.TransferAsync(ClientId, id, dto.RecipientAccountId, dto.Amount);
            if (result.Failed)
                return Failure(result);
            AppDTO.TransferTransactionDTO trans = result.Value;

            return Ok(new ApiDTO.TransferTransactionDTO
            {
                CreatedAt = trans.CreatedAt,
                Amount = trans.Amount,
                SenderAccountCode = trans.SenderAccountCode,
                RecipientAccountCode = trans.RecipientAccountCode
            });
        }
    }
}
