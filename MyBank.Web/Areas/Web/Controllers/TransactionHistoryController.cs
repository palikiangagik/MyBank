using Microsoft.AspNetCore.Mvc;
using MyBank.Web.Areas.Web.ViewModels;
using MyBank.Web.Contracts.Account;
using System.Linq;
using System.Threading.Tasks;
using MyBank.Web.ViewModels;


namespace MyBank.Web.Areas.Web.Controllers
{
    public class TransactionHistoryController : BaseController
    {
        private readonly IAccountService _accountService;

        public TransactionHistoryController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 10;

            var result = await _accountService.GetTransactionHistoryAsync(UserNameIdentifier,
                page, pageSize);

            if (!result.IsSuccess)
                return Failure(result);

            var transactions = result.Value.Items
                .Select(trans => new TransactionHistoryViewItem {
                    Type = trans.Type,
                    CreatedAt = trans.CreatedAt,
                    Amount = trans.Amount,
                    SenderAccountCode = trans.SenderAccountCode,
                    SenderName = trans.SenderUserName,
                    RecipientAccountCode = trans.RecepientAccountCode,
                    RecipientName = trans.RecipientUserName
                }).ToList();

            return View(new TransactionHistoryViewModel {
                Transactions = transactions,
                PageViewModel = new PageViewModel(result.Value.TotalCount, page, pageSize)
            });            
        }
    }
}
