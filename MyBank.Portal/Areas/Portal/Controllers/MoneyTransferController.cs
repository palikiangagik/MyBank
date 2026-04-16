using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyBank.Portal.Areas.Portal.ViewModels;
using MyBank.Portal.Contracts.Account;
using MyBank.Portal.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace MyBank.Portal.Areas.Portal.Controllers
{
    public class MoneyTransferController : BaseController
    {
        private readonly IAccountService _accountService;

        public MoneyTransferController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<IActionResult> Index()
        {
            return await RefillAndReturn(new MoneyTransferViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(MoneyTransferViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return await RefillAndReturn(viewModel, nameof(Index));

            var result = await _accountService.TransferMoneyAsync(UserNameIdentifier, viewModel.FromAccount,
                viewModel.ToAccount, viewModel.Amount);

            if (!result.IsSuccess)
                return await RefillAndReturn(viewModel, nameof(Index), result);
            
            // TODO: consider using other approach
            TempData["Message"] = $"{viewModel.Amount} sent successfully";
            return RedirectToAction(nameof(Index));            
        }

        private async Task<IActionResult> RefillAndReturn(MoneyTransferViewModel viewModel,
            string action = null,
            Result result = null)
        {
            // TODO: add pagination
            var accountFromResult = await _accountService.GetAccountsAsync(UserNameIdentifier, 1, int.MaxValue);

            if (!accountFromResult.IsSuccess)
                return Failure(accountFromResult, null, action);

            // TODO: add pagination
            var accountToResult = await _accountService.GetDestinationAccountsAsync(UserNameIdentifier, 1, int.MaxValue);

            if (!accountToResult.IsSuccess)
                return Failure(accountToResult, null, action);

            viewModel.FromAccounts = accountFromResult.Value?.Items
            .Select(acc => new SelectListItem
            {
                Value = acc.Id.ToString(),
                Text = $"{acc.Id} (Balance: {acc.Balance})" // TODO: move to a helper method
            }).ToList();

            viewModel.ToAccounts = accountToResult.Value?.Items
            .Select(acc => new SelectListItem
            {
                Value = acc.Id.ToString(),
                Text = $"{acc.UserName} - {acc.Id}" // TODO: move to a helper method
            }).ToList();

            if (null != result && !result.IsSuccess)
                return Failure(result, viewModel, action);
            else if (!string.IsNullOrEmpty(action))
                return View(action, viewModel);
            else
                return View(viewModel);
        }
    }
}
