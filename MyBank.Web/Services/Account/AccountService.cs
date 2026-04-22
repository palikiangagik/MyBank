using Microsoft.EntityFrameworkCore;
using MyBank.Web.Contracts.Account;
using MyBank.Web.Contracts.Account.DTO;
using MyBank.Web.Data;
using MyBank.Web.Data.Models;
using MyBank.Web.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace MyBank.Web.Services.Account
{
    public class AccountService : IAccountService
    {
        private readonly MyBankWebContext _context;

        public AccountService(MyBankWebContext context) 
        { 
            _context = context;
        }

        public async Task<Result<AccountListDTO>> GetAccountsAsync(string currentUserId, int page, int pageSize)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == currentUserId);
            if (!userExists)
                return Errors.UserNotFound;

            if (page < 1 || pageSize < 1)
                return Errors.InvalidPagingParameters; 

            // TODO: move to the stored procedure for performance

            var query = _context.Accounts
                .Where(acc => acc.UserId == currentUserId && !acc.IsClosed);

            var totalCount = await query.CountAsync();

            var totalBalance = await query.SumAsync(acc => acc.Balance);

            var items = await query
                .OrderBy(acc => acc.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(acc => new AccountListItemDTO
                {
                    Id = acc.Id,
                    Code = acc.Code,
                    Balance = acc.Balance
                }).ToListAsync();

            return new AccountListDTO
            {
                Items = items,
                TotalCount = totalCount,
                TotalBalance = totalBalance
            };            
        }

        public async Task<Result<AccountDTO>> GetAccount(string currentUserId, int accountId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == currentUserId);
            if (!userExists)
                return Errors.UserNotFound;

            var account = await _context.Accounts
                .FirstAsync(acc => acc.Id == accountId && !acc.IsClosed && acc.UserId == currentUserId);

            if (null == account)
                return Errors.AccountNotFound;

            return new AccountDTO
            {
                Id = account.Id,
                Code = account.Code,
                Balance = account.Balance
            };
        }

        public async Task<Result<DestinationAccountListDTO>> GetDestinationAccountsAsync(
            string currentUserId, int page, int pageSize)
        {
            // TODO: implement friend destination account list for currentUserId

            var userExists = await _context.Users.AnyAsync(u => u.Id == currentUserId);
            if (!userExists)
                return Errors.UserNotFound;

            if (page < 1 || pageSize < 1)
                return Errors.InvalidPagingParameters;

            var query = _context.Accounts.Where(acc => !acc.IsClosed);

            var totalCount = await query.CountAsync();

            // TODO: maybe add CancellationToken  ? 

            var items = await query
                .OrderBy(acc => acc.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(acc => new DestinationAccountListItemDTO{
                    Id = acc.Id,
                    UserName = acc.User.UserName,
                    Code = acc.Code                    
                })
                .ToListAsync();

            return new DestinationAccountListDTO
            {
                Items = items,
                TotalCount = totalCount,
            };
        }

        public async Task<Result<AccountOpenDTO>> OpenNewAccountAsync(string currentUserId, decimal balance)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == currentUserId);
            if (!userExists)
                return Errors.UserNotFound;

            if (balance < 0)
                return Errors.NegativeAmount;
            
            var account = new Data.Models.Account
            {
                UserId = currentUserId,
                Balance = balance
            };

            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
            return new AccountOpenDTO { Id = account.Id };
        }

        public async Task<Result> CloseAccountAsync(string currentUserId, int accountId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == currentUserId);
            if (!userExists)
                return Errors.UserNotFound;

            Data.Models.Account account = await _context.Accounts
                .Where(acc => acc.UserId == currentUserId && acc.Id == accountId && !acc.IsClosed)
                .FirstOrDefaultAsync();

            if (null == account)
                return Errors.AccountNotFound;

            if (account.Balance > 0)
                return Errors.ClosureDeniedWithBalance;

            account.IsClosed = true;
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result<AccountDepositDTO>> DepositAsync(string currentUserId, int accountId, decimal amount)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == currentUserId);
            if (!userExists)
                return Errors.UserNotFound;

            //TODO: add transaction

            if (amount <= 0)
                return Errors.NonPositiveAmount;

            Data.Models.Account account = await _context.Accounts
                .Where(acc => acc.UserId == currentUserId && acc.Id == accountId && !acc.IsClosed) // TODO: move acc.IsClosed to a common place, e.g. a global query filter
                .FirstOrDefaultAsync();

            if (null == account)
                return Errors.AccountNotFound;

            await _context.Transactions.AddAsync(new Transaction
            {
                Type = TransactionType.Deposit,
                Amount = amount,
                Sender = null,
                Recipient = account
            });

            account.Balance += amount;

            await _context.SaveChangesAsync();
            return new AccountDepositDTO
            {
                Code = account.Code,
                Amount = amount
            };
        }

        public async Task<Result<AccountWithdrawDTO>> WithdrawAsync(string currentUserId, int accountId, decimal amount)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == currentUserId);
            if (!userExists)
                return Errors.UserNotFound;

            //TODO: add transaction

            if (amount <= 0)
                return Errors.NonPositiveAmount;

            Data.Models.Account account = await _context.Accounts
                .Where(acc => acc.UserId == currentUserId && acc.Id == accountId && !acc.IsClosed) // TODO: move acc.IsClosed to a common place, e.g. a global query filter
                .FirstOrDefaultAsync();

            if (null == account)
                return Errors.AccountNotFound;

            if (amount > account.Balance)
                return Errors.NotEnoughBalance;

            await _context.Transactions.AddAsync(new Transaction
            {
                Type = TransactionType.Withdrawal,
                Amount = amount,
                Sender = account,
                Recipient = null
            });

            account.Balance -= amount;

            await _context.SaveChangesAsync();
            return new AccountWithdrawDTO
            {
                Code = account.Code,
                Amount = amount
            };
        }


        public async Task<Result<TransferMoneyDTO>> TransferMoneyAsync(string currentUserId, int accountFromId, 
            int accountToId, decimal amount)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == currentUserId);
            if (!userExists)
                return Errors.UserNotFound;

            if (amount <= 0)
                return Errors.NonPositiveAmount;

            if (accountFromId == accountToId)
                return Errors.SelfTransferNotAllowed;

            var accounts = await _context.Accounts
                .Include(acc => acc.User)
                .Where(acc =>
                    !acc.IsClosed &&
                    ((acc.UserId == currentUserId && acc.Id == accountFromId) || acc.Id == accountToId)
                )
                .ToListAsync();

            var accountFrom = accounts.FirstOrDefault(acc => acc.Id == accountFromId);
            var accountTo = accounts.FirstOrDefault(acc => acc.Id == accountToId);

            if (accountFrom == null || accountTo == null)
                return Errors.AccountNotFound;

            if (amount > accountFrom.Balance)
                return Errors.NotEnoughBalance;

            accountFrom.Balance -= amount;
            accountTo.Balance += amount;

            _context.Transactions.Add(new Transaction
            {
                Type = TransactionType.Transfer,
                Amount = amount,
                Sender = accountFrom,
                Recipient = accountTo
            });

            await _context.SaveChangesAsync();
            return new TransferMoneyDTO
            {
                SenderCode = accountFrom.Code,
                RecepientCode = accountTo.Code,
                RecepientUserName = accountTo.User?.UserName,
                Amount = amount
            };
        }


        public async Task<Result<TransactionListDTO>> GetTransactionHistoryAsync(string currentUserId, 
            int page, int pageSize)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == currentUserId);
            if (!userExists)
                return Errors.UserNotFound;

            if (page < 1 || pageSize < 1)
                return Errors.InvalidPagingParameters;

            var query =  _context.Transactions
                .Where(trans => trans.Recipient.UserId == currentUserId || trans.Sender.UserId == currentUserId);

            var totalCount = await query.CountAsync();

            var transactions = await query
                .OrderByDescending(trans => trans.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(trans => new TransactionListItemDTO
                {
                    Type = trans.Type,
                    CreatedAt = trans.CreatedAt,
                    Amount = trans.Amount,
                    SenderAccountCode = trans.Sender.Code,
                    SenderUserName = trans.Sender.User.UserName,
                    RecepientAccountCode = trans.Recipient.Code,
                    RecipientUserName = trans.Recipient.User.UserName
                })
                .ToListAsync();

            return new TransactionListDTO {
                Items = transactions,
                TotalCount = totalCount
            };
        }
    }
}
