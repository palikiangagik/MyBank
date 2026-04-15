using Microsoft.EntityFrameworkCore;
using MyBank.Portal.Contracts.Account;
using MyBank.Portal.Data;
using MyBank.Portal.Infrastructure;
using MyBank.Portal.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MyBank.Portal.Services.Account
{
    public class AccountService : IAccountService
    {
        private readonly MyBankPortalContext _context;

        public AccountService(MyBankPortalContext context) 
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
                    Balance = acc.Balance
                }).ToListAsync();

            return new AccountListDTO
            {
                Items = items,
                TotalCount = totalCount,
                TotalBalance = totalBalance
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
                    UserName = acc.User.UserName
                })
                .ToListAsync();

            return new DestinationAccountListDTO
            {
                Items = items,
                TotalCount = totalCount,
            };
        }

        public async Task<Result> OpenNewAccountAsync(string currentUserId, decimal balance)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == currentUserId);
            if (!userExists)
                return Errors.UserNotFound;

            if (balance < 0)
                return Errors.NegativeAmount;
            
            await _context.Accounts.AddAsync(
                new  Models.Account {
                    UserId = currentUserId,
                    Balance = balance
                }
            );

            await _context.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> CloseAccountAsync(string currentUserId, int accountId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == currentUserId);
            if (!userExists)
                return Errors.UserNotFound;

            Models.Account account = await _context.Accounts
                .Where(acc => acc.UserId == currentUserId && acc.Id == accountId && !acc.IsClosed)
                .FirstOrDefaultAsync();

            if (null == account)
                return Errors.AccountNotFound;

            account.IsClosed = true;
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> DepositAsync(string currentUserId, int accountId, decimal amount)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == currentUserId);
            if (!userExists)
                return Errors.UserNotFound;

            //TODO: add transaction

            if (amount <= 0)
                return Errors.NonPositiveAmount;

            Models.Account account = await _context.Accounts
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
            return Result.Success();
        }

        public async Task<Result> WithdrawAsync(string currentUserId, int accountId, decimal amount)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == currentUserId);
            if (!userExists)
                return Errors.UserNotFound;

            //TODO: add transaction

            if (amount <= 0)
                return Errors.NonPositiveAmount;

            Models.Account account = await _context.Accounts
                .Where(acc => acc.UserId == currentUserId && acc.Id == accountId && !acc.IsClosed) // TODO: move acc.IsClosed to a common place, e.g. a global query filter
                .FirstOrDefaultAsync();

            if (null == account)
                return Errors.AccountNotFound;

            if (amount > account.Balance)
                return Errors.NotEnoughBalance;

            await _context.Transactions.AddAsync(new Transaction
            {
                Type = TransactionType.Deposit,
                Amount = amount,
                Sender = null,
                Recipient = account
            });

            account.Balance -= amount;

            await _context.SaveChangesAsync();
            return Result.Success();
        }


        public async Task<Result> TransferMoneyAsync(string currentUserId, int accountFromId, 
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
            return Result.Success();    
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
                    SenderAccountId = trans.SenderId,
                    SenderUserName = trans.Sender.User.UserName,
                    RecipientAccountId = trans.RecipientId,
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
