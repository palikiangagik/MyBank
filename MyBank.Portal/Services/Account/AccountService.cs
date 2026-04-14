using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyBank.Portal.Contracts.Account;
using MyBank.Portal.Data;
using MyBank.Portal.Infrastructure;
using MyBank.Portal.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyBank.Portal.Services.Account
{
    public class AccountService : IAccountService
    {
        private readonly MyBankPortalContext _context;
        private readonly ILogger<AccountService> _logger;

        

        public AccountService(MyBankPortalContext context, ILogger<AccountService> logger) 
        { 
            _context = context;
            _logger = logger;
        }

        private void ValidateCurrentUserId(string currentUserId)
        {
            if (string.IsNullOrEmpty(currentUserId))
            {                
                _logger.LogError("Attempt to get accounts for null or empty userId.");
                throw new ArgumentNullException(nameof(currentUserId), "User ID is required for this operation.");
            }
        }

        private void ValidatePagingParameters(int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                _logger.LogWarning($"Invalid paging parameters received. Page: {page}, Size: {pageSize}. Resetting to defaults.");
                throw new ArgumentOutOfRangeException(nameof(page), "Page number and page size must be greater than zero.");
            }
        }

        public async Task<Result<AccountListDTO>> GetAccounts(string currentUserId, int page, int pageSize)
        {
            ValidatePagingParameters(page, pageSize);
            ValidateCurrentUserId(currentUserId);

            try
            {
                var query = _context.Accounts
                    .Where(acc => acc.User.Id == currentUserId && !acc.IsClosed);

                var totalCount = await query.CountAsync();

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
                    TotalCount = totalCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting accounts for user {currentUserId}. Page: {page}, Size: {pageSize}.");
                throw; 
            }
        }

        public async Task<Result> Deposit(string currentUserId, int accountId, decimal amount)
        {
            ValidateCurrentUserId(currentUserId);

            //TODO: add transaction

            if (amount <= 0)
                return Errors.NonPositiveAmount;

            try
            {
                Models.Account account = await _context.Accounts
                    .Where(acc => acc.User.Id == currentUserId && acc.Id == accountId && !acc.IsClosed) // TODO: move acc.IsClosed to a common place, e.g. a global query filter
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error depositing amount for user {currentUserId} into account {accountId}.");
                throw;
            }

            return Result.Success();
        }

    }
}
