using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyBank.Web.Areas.Web.ViewModels;
using System.Threading.Tasks;

namespace MyBank.Web.Areas.Web.Controllers
{
    public class MoneyTransferController : BaseController
    {
        //private readonly IAccountService _accountService;

        //public MoneyTransferController(IAccountService accountService)
        //{
        //    _accountService = accountService;
        //}

        public async Task<IActionResult> Index()
        {
            return View();//return await RefillAndReturn(new MoneyTransferViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(MoneyTransferViewModel viewModel)
        {
            //if (!ModelState.IsValid)
            //    return await RefillAndReturn(viewModel, nameof(Index));

            //var result = await _accountService.TransferMoneyAsync(UserNameIdentifier, viewModel.FromAccount,
            //    viewModel.ToAccount, viewModel.Amount);

            //if (!result.IsSuccess)
            //    return await RefillAndReturn(viewModel, nameof(Index), result);
            
            //// TODO: consider using other approach
            //TempData["Message"] = $"{result.Value.Amount} sent successfully from " +
            //    $"{result.Value.SenderCode} to {result.Value.RecepientCode} ({result.Value.RecepientUserName}).";
            return RedirectToAction(nameof(Index));            
        }

        //private async Task<IActionResult> RefillAndReturn(MoneyTransferViewModel viewModel,
        //    string action = null,
        //    Result result = null)
        //{
        //    // TODO: add pagination
        //    var accountFromResult = await _accountService.GetAccountsAsync(UserNameIdentifier, 1, int.MaxValue);

        //    if (!accountFromResult.IsSuccess)
        //        return Failure(accountFromResult, null, action);

        //    // TODO: add pagination
        //    var accountToResult = await _accountService.GetDestinationAccountsAsync(UserNameIdentifier, 1, int.MaxValue);

        //    if (!accountToResult.IsSuccess)
        //        return Failure(accountToResult, null, action);

        //    viewModel.FromAccounts = accountFromResult.Value?.Items
        //    .Select(acc => new SelectListItem
        //    {
        //        Value = acc.Id.ToString(),
        //        Text = $"{acc.Code} (Balance: {acc.Balance})" // TODO: move to a helper method
        //    }).ToList();

        //    viewModel.ToAccounts = accountToResult.Value?.Items
        //    .Select(acc => new SelectListItem
        //    {
        //        Value = acc.Id.ToString(),
        //        Text = $"{acc.Code} ({acc.UserName})" // TODO: move to a helper method
        //    }).ToList();

        //    if (null != result && !result.IsSuccess)
        //        return Failure(result, viewModel, action);
        //    else if (!string.IsNullOrEmpty(action))
        //        return View(action, viewModel);
        //    else
        //        return View(viewModel);
        //}
    }
}
