using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyBank.Web.Areas.Web.ViewModels;
using MyBank.Web.Contracts.Account;
using MyBank.Web.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace MyBank.Web.Areas.Web.Controllers
{
    public class DepositAndWithdrawalController : BaseController
    {
        private readonly IAccountService _accountService;

        public DepositAndWithdrawalController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<IActionResult> Index()
        {
            return await RefillAndReturn(new DepositAndWithdrawalViewModel());
        }        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deposit(DepositAndWithdrawalViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return await RefillAndReturn(viewModel, nameof(Index));            

            var result = await _accountService.DepositAsync(UserNameIdentifier,
                viewModel.Account, viewModel.Amount);

            if (!result.IsSuccess)
                return await RefillAndReturn(viewModel, nameof(Index), result);

            // TODO: +add IStringLocalizer
            // TODO: consider using a more robust notification system for user feedback 
            TempData["Message"] = $"Deposited {result.Value.Amount} to account {result.Value.Code} successfully."; 
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(DepositAndWithdrawalViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return await RefillAndReturn(viewModel, nameof(Index));

            var result = await _accountService.WithdrawAsync(UserNameIdentifier,
                viewModel.Account, viewModel.Amount);

            if (!result.IsSuccess)
                return await RefillAndReturn(viewModel, nameof(Index), result);

            TempData["Message"] = $"Withdrew {result.Value.Amount} from account {result.Value.Code} successfully.";
            return RedirectToAction(nameof(Index));
        }


        private async Task<IActionResult> RefillAndReturn(DepositAndWithdrawalViewModel viewModel, 
            string action = null,
            Result result = null
        )
        {
            // TODO: add pagination
            var getResult = await _accountService.GetAccountsAsync(
               UserNameIdentifier,
               1,
               int.MaxValue // TODO: add pagination support
           );

            if (!getResult.IsSuccess)
                return Failure(getResult, null, action);

            viewModel.Accounts = getResult.Value?.Items.Select(acc => new SelectListItem
            {
                Value = acc.Id.ToString(),
                Text = $"{acc.Code} (Balance: {acc.Balance})"
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
