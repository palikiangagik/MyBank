using Microsoft.AspNetCore.Mvc;
using MyBank.Web.Areas.Web.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using MyBank.Application.UseCases;
using MyBank.Application.DTO;

namespace MyBank.Web.Areas.Web.Controllers
{
    public class ProfileController : BaseController
    {
        private readonly ClientUseCases _clientUseCases;
        private readonly AccountsUseCases _accountsUseCases;

        public ProfileController(ClientUseCases clientUseCases, AccountsUseCases accountsUseCases)
        {
            _clientUseCases = clientUseCases;
            _accountsUseCases = accountsUseCases;
        }

        public async Task<IActionResult> Index()
        {
            var summaryResult = await _clientUseCases.GetClientSummaryAsync(ClientId, new(1, int.MaxValue));
            if (summaryResult.Failed)
                return Failure(summaryResult);
            ClientSummaryDTO summary = summaryResult.Value; 

            return View(new ProfileViewModel
            {
                UserName = summary.Name.FirstName + " " + summary.Name.LastName,
                Balance = summary.TotalBalance,
                Accounts = summary.AccountList.Items
                .Select(acc => new ProfileAccountViewItem
                {
                    Id = acc.Id,
                    Code = acc.Code,
                    Balance = acc.Balance
                }).ToList()
            });
        }

        public async Task<IActionResult> OpenNewAccount()
        {
            return View(new OpenNewAccountViewModel());            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OpenNewAccount(OpenNewAccountViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var result = await _accountsUseCases.OpenAccountAsync(ClientId, viewModel.Amount);

            if (result.Failed)
                return Failure(result, viewModel);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CloseAccount(int id)
        {
            var result = await _accountsUseCases.GetAccountSummaryAsync(ClientId, id);

            if (result.Failed)
                return Failure(result);
            AccountSummaryDTO dto = result.Value;

            return View(new CloseAccountViewModel
            {
                Id = dto.Id,
                Code = dto.Code,
                Balance = dto.Balance
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseAccount(CloseAccountViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var result = await _accountsUseCases.CloseAccountAsync(ClientId, viewModel.Id);

            if (result.Failed)
                return Failure(result, viewModel);

            return RedirectToAction(nameof(Index));
        }
    }
}
