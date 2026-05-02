using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyBank.Application.UseCases;
using MyBank.Web.Areas.Web.ViewModels;
using System.Threading.Tasks;
using CorePrimitives;
using System.Linq;

namespace MyBank.Web.Areas.Web.Controllers
{
    public class MoneyTransferController : BaseController
    {
        private readonly AccountsUseCases _accountsUseCases;

        public MoneyTransferController(AccountsUseCases accountsUseCases)
        {
            _accountsUseCases = accountsUseCases;
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

            var result = await _accountsUseCases.TransferAsync(ClientId, viewModel.FromAccount,
                viewModel.ToAccount, viewModel.Amount);

            if (result.Failed)
                return await RefillAndReturn(viewModel, nameof(Index), result);

            TempData["Message"] = $"{result.Value.Amount} sent successfully from " +
                $"{result.Value.SenderAccountCode} to {result.Value.RecipientAccountCode}.";
            return RedirectToAction(nameof(Index));            
        }

        private async Task<IActionResult> RefillAndReturn(MoneyTransferViewModel viewModel,
            string action = null,
            Result result = null)
        {
            var accountsFrom = await _accountsUseCases.GetClientAccountListAsync(ClientId, new(1, int.MaxValue));
            var accountsTo = await _accountsUseCases.GetDestinationAccountListAsync(ClientId, new(1, int.MaxValue));

            viewModel.FromAccounts = accountsFrom.Items
            .Select(acc => new SelectListItem
            {
                Value = acc.Id.ToString(),
                Text = $"{acc.Code} (Balance: {acc.Balance})"
            }).ToList();

            viewModel.ToAccounts = accountsTo.Items
            .Select(acc => new SelectListItem
            {
                Value = acc.Id.ToString(),
                Text = $"{acc.Code} ({acc.Name.FirstName} {acc.Name.LastName})" 
            }).ToList();

            if (result is not null && result.Failed)
                return Failure(result, viewModel, action);
            else if (!string.IsNullOrEmpty(action))
                return View(action, viewModel);
            else
                return View(viewModel);
        }
    }
}
