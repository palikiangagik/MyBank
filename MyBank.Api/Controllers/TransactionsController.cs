using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBank.Application.UseCases;
using MyBank.Domain.Entities;

using ApiDTO = MyBank.Api.DTO;
using AppDTO = MyBank.Application.DTO;

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
        public async Task<IActionResult> GetTransactions([FromQuery] ApiDTO.PagingParametersDTO dto)
        {
            var result = await _transactionsUseCases.GetTransactionHistoryAsync(ClientId, new(dto.Page, dto.PageSize));

            var items = result.Items.Select(trans => new ApiDTO.TransactionHistoryDTO.Item
            {
                Type = trans.Type switch
                {
                    TransactionType.Transfer => "Transfer",
                    TransactionType.Withdrawal => "Withdrawal",
                    TransactionType.Deposit => "Deposit",
                    _ => "unknown"
                },
                CreatedAt = trans.CreatedAt,
                Amount = trans.Amount,
                AccountCode = trans.AccountCode,
                SenderAccountCode = trans.Sender?.AccountCode,
                SenderName = trans.Sender is null ? null : trans.Sender.FirstName + " " + trans.Sender.LastName,
                RecipientAccountCode = trans.Recipient?.AccountCode,
                RecipientName = trans.Recipient is null ? null : trans.Recipient.FirstName + " " + trans.Recipient.LastName
            }).ToList();

            return Ok(new ApiDTO.TransactionHistoryDTO
            {
                TotalCount = result.TotalCount,
                Items = items
            });                
        }
    }
}