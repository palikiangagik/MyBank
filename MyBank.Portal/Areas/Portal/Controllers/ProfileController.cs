using Microsoft.AspNetCore.Mvc;
using MyBank.Portal.Areas.Portal.ViewModels;
using MyBank.Portal.Contracts.Account;
using System.Linq;
using System.Threading.Tasks;

namespace MyBank.Portal.Areas.Portal.Controllers
{
    public class ProfileController : BaseController
    {
        private readonly  IAccountService _accountService;

        public ProfileController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<IActionResult> Index()
        {            
            var result = await _accountService.GetAccountsAsync(
                UserNameIdentifier, 
                1, 
                int.MaxValue // TODO: add pagination support
            );

            if (!result.IsSuccess)
                return Failure(result);

            return View(new ProfileViewModel {
                UserName = UserName,
                Balance = result.Value.TotalBalance,
                Accounts = result.Value
                .Items
                .Select(acc => new ProfileAccountViewItem
                {
                    Id = acc.Id,
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

            var result = await _accountService.OpenNewAccountAsync(UserNameIdentifier, viewModel.Amount);

            if (!result.IsSuccess)
                return Failure(result, viewModel);            

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CloseAccount(int id)
        {
            return View(new CloseAccountViewModel { Id = id });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseAccount(CloseAccountViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var result = await _accountService.CloseAccountAsync(UserNameIdentifier, viewModel.Id);

            if (!result.IsSuccess)
                return Failure(result, viewModel);

            return RedirectToAction(nameof(Index));
        }
    }
}
