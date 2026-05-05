using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CorePrimitives;
using System.Linq;
using MyBank.Application.UseCases;
using MyBank.Web.Areas.Web.ViewModels;
using MyBank.Web.ViewModels;

namespace MyBank.Web.Areas.Web.Controllers
{
    public class TransactionHistoryController : BaseController
    {
        private readonly TransactionsUseCases _transactionsUseCases;

        public TransactionHistoryController(TransactionsUseCases transactionsUseCases)
        {
            _transactionsUseCases = transactionsUseCases;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 10;

            var result = await _transactionsUseCases.GetTransactionHistoryAsync(ClientId, new(page, pageSize));

            var transactions = result.Items
                .Select(trans => new TransactionHistoryViewItem
                {
                    Type = trans.Type,
                    CreatedAt = trans.CreatedAt,
                    Amount = trans.Amount,
                    AccountCode = trans.AccountCode,
                    SenderAccountCode = trans.Sender?.AccountCode,
                    SenderName = trans.Sender is null ? null : trans.Sender.Name.FirstName + " " + trans.Sender.Name.LastName,
                    RecipientAccountCode = trans.Recipient?.AccountCode,
                    RecipientName = trans.Recipient is null ? null : trans.Recipient.Name.FirstName + " " + trans.Recipient.Name.LastName
                }).ToList();

            return View(new TransactionHistoryViewModel
            {
                Transactions = transactions,
                PageViewModel = new PageViewModel(result.TotalCount, page, pageSize)
            });            
        }
    }
}
