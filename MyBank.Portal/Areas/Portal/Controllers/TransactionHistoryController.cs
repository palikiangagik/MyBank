using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBank.Portal.Areas.Portal.ViewModels;
using MyBank.Portal.Data;
using System.Linq;
using System.Threading.Tasks;


namespace MyBank.Portal.Areas.Portal.Controllers
{
    public class TransactionHistoryController : BaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly MyBankPortalContext _context;

        public TransactionHistoryController(UserManager<IdentityUser> userManager, MyBankPortalContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Problem("User not found");

            var transactions = await _context.Transactions
                .Where(acc => acc.Recipient.User == user || acc.Sender.User == user)
                .Select(acc => new TransactionHistoryViewItem
                {
                    Type = acc.Type,
                    CreatedAt = acc.CreatedAt,
                    Amount = acc.Amount,
                    Sender = new TransactionHistoryAccountViewItem
                    {
                        Name = acc.Sender.User.UserName,
                        Id = acc.Sender.Id
                    },
                    Recipient = new TransactionHistoryAccountViewItem
                    {
                        Name = acc.Recipient.User.UserName,
                        Id = acc.Recipient.Id
                    }
                })
                .ToListAsync();

            return View(new TransactionHistoryViewModel
            {
                Transactions = transactions
            });
        }

    }
}
