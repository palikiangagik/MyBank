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
        private readonly ProfileUseCases _profileUseCases;
        private readonly AccountsUseCases _accountsUseCases;

        public ProfileController(ProfileUseCases profileUseCases, AccountsUseCases accountsUseCases)
        {
            _profileUseCases = profileUseCases;
            _accountsUseCases = accountsUseCases;
        }

        public async Task<IActionResult> Index()
        {
            var summary = await _profileUseCases.GetProfileSummaryAsync(UserNameIdentifier, 1, int.MaxValue);

            return View(new ProfileViewModel
            {
                UserName = UserName,
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

            var result = await _accountsUseCases.OpenAccountAsync(UserNameIdentifier, viewModel.Amount);

            if (!result.IsSuccess)
                return Failure(result, viewModel);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CloseAccount(int id)
        {
            var result = await _accountsUseCases.GetAccountSummaryAsync(UserNameIdentifier, id);

            if (!result.IsSuccess)
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

            var result = await _accountsUseCases.CloseAccountAsync(UserNameIdentifier, viewModel.Id);

            if (!result.IsSuccess)
                return Failure(result, viewModel);

            return RedirectToAction(nameof(Index));
        }
    }
}
