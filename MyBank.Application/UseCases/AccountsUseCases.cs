using CorePrimitives;
using MyBank.Domain.Entities;
using MyBank.Application.Interfaces;
using MyBank.Domain.Services;
using MyBank.Application.Helpers;
using MyBank.Application.DTO.Accounts;
using MyBank.Application.DTO.Transactions;
using MyBank.Application.DTO.Common;

namespace MyBank.Application.UseCases
{
    public class AccountsUseCases
    {
        private readonly AccountService _accountService; // not iface, bc its not external detail
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountQuerier _accountQuerier;

        private readonly ITransactionRepository _transactionRepository;

        private readonly IDbSession _db;

        public AccountsUseCases(AccountService accountService, IAccountRepository accountRepository,
            IAccountQuerier accountQuerier, ITransactionRepository transactionRepository, IDbSession db)
        {
            _accountService = accountService;
            _accountRepository = accountRepository;
            _accountQuerier = accountQuerier;
            _transactionRepository = transactionRepository;
            _db = db;
        }


        public async Task<Result<AccountSummaryDTO>> GetAccountSummaryAsync(int clientId, int accountId) =>
            await _accountQuerier.GetAccountSummaryAsync(clientId, accountId);

        public async Task<AccountSummaryListDTO> GetClientAccountListAsync(int clientId, PagingParametersDTO pagingParameters) =>
            await _accountQuerier.GetClientAccountListAsync(clientId, pagingParameters);

        public async Task<DestinationAccountListDTO> GetDestinationAccountListAsync(int clientId, PagingParametersDTO pagingParameters) =>
            await _accountQuerier.GetDestinationAccountListAsync(clientId, pagingParameters);

        public async Task<Result<AccountSummaryDTO>> OpenAccountAsync(int clientId, decimal balance)
        {
            await using var trx = await TransactionScope.StartAsync(_db);

            var result = await _accountService.OpenAccountAsync(clientId, balance);
            if (result.Failed)
                return result.Error!;
            var (account, transaction) = result.Value;

            await _accountRepository.AddAsync(account);
            if (transaction is not null)
                await _transactionRepository.AddAsync(transaction);

            await trx.CommitAsync();
            return new AccountSummaryDTO{
                Id = account.Id, 
                Code = account.Code, 
                Balance = account.Balance 
            };
        }

        public async Task<Result> CloseAccountAsync(int clientId, int accountId)
        {
            await using var trx = await TransactionScope.StartAsync(_db);
            Result<Account> getResult = await _accountRepository.GetAsync(clientId, accountId, true);
            if (getResult.Failed)
                return getResult;
            Account account = getResult.Value;

            Result closeResult = account.Close();
            if (closeResult.Failed)
                return closeResult;

            await _accountRepository.UpdateAsync(account);
            await trx.CommitAsync();
            return Result.Success();
        }

        public async Task<Result<WithdrawalTransactionDTO>> WithdrawAsync(int clientId, int accountId, decimal amount)
        {
            await using var trx = await TransactionScope.StartAsync(_db);

            Result<Account> getResult = await _accountRepository.GetAsync(clientId, accountId, true);
            if (getResult.Failed)
                return getResult.Error!;
            Account account = getResult.Value;

            Result<WithdrawalTransaction> withdrawResult = await _accountService.WithdrawAsync(account, amount);
            if (withdrawResult.Failed)
                return withdrawResult.Error!;
            WithdrawalTransaction transaction = withdrawResult.Value;

            await _transactionRepository.AddAsync(transaction);
            await _accountRepository.UpdateAsync(account);

            await trx.CommitAsync();
            return new WithdrawalTransactionDTO{
                CreatedAt = transaction.CreatedAt,
                Amount = transaction.Amount,
                AccountCode = account.Code
            };
        }

        public async Task<Result<DepositTransactionDTO>> DepositAsync(int clientId, int accountId, decimal amount)
        {
            await using var trx = await TransactionScope.StartAsync(_db);

            Result<Account> getResult = await _accountRepository.GetAsync(clientId, accountId, true);
            if (getResult.Failed)
                return getResult.Error!;
            Account account = getResult.Value;

            Result<DepositTransaction> depositResult = await _accountService.DepositAsync(account, amount);
            if (depositResult.Failed)
                return depositResult.Error!;
            DepositTransaction transaction = depositResult.Value;

            await _transactionRepository.AddAsync(transaction);
            await _accountRepository.UpdateAsync(account);

            await trx.CommitAsync();
            return new DepositTransactionDTO{
                CreatedAt = transaction.CreatedAt, 
                Amount = transaction.Amount, 
                AccountCode = account.Code
            };
        }

        public async Task<Result<TransferTransactionDTO>> TransferAsync(int clientId,
            int sourceAccountId, int recipientAccountId, decimal amount)
        {
            await using var trx = await TransactionScope.StartAsync(_db);

            Result<Account> getSourceResult = await _accountRepository.GetAsync(clientId, sourceAccountId, true);
            if (getSourceResult.Failed)
                return getSourceResult.Error!;//return getSourceResult.Errors;
            Account senderAccount = getSourceResult.Value;

            Result<Account> getDestinationResult = await _accountRepository.GetAsync(recipientAccountId, true);
            if (getDestinationResult.Failed)
                return getDestinationResult.Error!;//return getDestinationResult.Errors;
            Account recipientAccount = getDestinationResult.Value;
             
            Result<TransferTransaction> transferResult = await _accountService.TransferAsync(senderAccount,
                recipientAccount, amount);
            if (transferResult.Failed)
                return transferResult.Error!;//return transferResult.Errors;
            TransferTransaction transaction = transferResult.Value;

            await _transactionRepository.AddAsync(transaction);
            await _accountRepository.UpdateAsync(senderAccount);
            await _accountRepository.UpdateAsync(recipientAccount);

            await trx.CommitAsync();
            return new TransferTransactionDTO{
                CreatedAt = transaction.CreatedAt, 
                Amount = transaction.Amount,
                SenderAccountCode = senderAccount.Code,
                RecipientAccountCode = recipientAccount.Code
            };
        }
    }
}