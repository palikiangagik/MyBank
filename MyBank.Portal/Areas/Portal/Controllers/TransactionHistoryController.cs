using Microsoft.AspNetCore.Mvc;
using MyBank.Portal.Areas.Portal.ViewModels;
using MyBank.Portal.Contracts.Account;
using System.Linq;
using System.Threading.Tasks;


namespace MyBank.Portal.Areas.Portal.Controllers
{
    public class TransactionHistoryController : BaseController
    {
        private readonly IAccountService _accountService;

        public TransactionHistoryController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<IActionResult> Index()
        {
            // TODO : add pagination
            var result = await _accountService.GetTransactionHistoryAsync(UserNameIdentifier,
                1, int.MaxValue);

            if (!result.IsSuccess)
                return Failure(result);

            var transactions = result.Value.Items
                .Select(trans => new TransactionHistoryViewItem {
                    Type = trans.Type,
                    CreatedAt = trans.CreatedAt,
                    Amount = trans.Amount,
                    SenderId = trans.SenderAccountId,
                    SenderName = trans.SenderUserName,
                    RecipientId = trans.RecipientAccountId,
                    RecipientName = trans.RecipientUserName
                }).ToList();

            return View(new TransactionHistoryViewModel {
                Transactions = transactions
            });            
        }
    }
}
