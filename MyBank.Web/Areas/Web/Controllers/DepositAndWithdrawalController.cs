using Microsoft.AspNetCore.Mvc;
using MyBank.Web.Areas.Web.ViewModels;
using System.Threading.Tasks;
using CorePrimitives;
using MyBank.Application.UseCases;
using MyBank.Application.DTO;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyBank.Web.Areas.Web.Controllers
{
    public class DepositAndWithdrawalController : BaseController
    {
        private readonly AccountsUseCases _accountsUseCases;

        public DepositAndWithdrawalController(AccountsUseCases accountsUseCases)
        {
            _accountsUseCases = accountsUseCases;
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

            var result = await _accountsUseCases.DepositAsync(UserNameIdentifier,
                viewModel.Account, viewModel.Amount);

            if (!result.IsSuccess)
                return await RefillAndReturn(viewModel, nameof(Index), result);

            TempData["Message"] = $"Deposited {result.Value.Amount} to account {result.Value.Code} successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(DepositAndWithdrawalViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return await RefillAndReturn(viewModel, nameof(Index));

            var result = await _accountsUseCases.WithdrawAsync(UserNameIdentifier,
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
            var accounts = await _accountsUseCases.GetUserAccountListAsync(
               UserNameIdentifier,
               1,
               int.MaxValue 
           );

            viewModel.Accounts = accounts.Items.Select(acc => new SelectListItem
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
