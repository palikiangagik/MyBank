using CorePrimitives;
using MyBank.Domain.Entities;
using MyBank.Application.DTO;
using MyBank.Application.Interfaces;
using MyBank.Domain.Services;
using MyBank.Domain.Interfaces;

namespace MyBank.Application.UseCases
{
    public class AccountsUseCases
    {
        private readonly AccountService _accountService; // not iface, bc its not external detail
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountQuerier _accountQuerier;

        private readonly ITransactionRepository _transactionRepository;

        private readonly IUnitOfWork _unitOfWork;

        public AccountsUseCases(AccountService accountService, IAccountRepository accountRepository,
            IAccountQuerier accountQuerier, ITransactionRepository transactionRepository, IUnitOfWork unitOfWork)
        {
            _accountService = accountService;
            _accountRepository = accountRepository;
            _accountQuerier = accountQuerier;
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
        }


        public async Task<Result<AccountSummaryDTO>> GetAccountSummaryAsync(string currentUserId, int accountId) =>
            await _accountQuerier.GetAccountSummaryAsync(currentUserId, accountId);

        public async Task<SubList<AccountSummaryDTO>> GetUserAccountListAsync(string currentUserId, int page, int pageSize) =>
            await _accountQuerier.GetUserAccountListAsync(currentUserId, page, pageSize);


        public async Task<SubList<DestinationAccountDTO>> GetDestinationAccountListAsync(
            string currentUserId, int page, int pageSize) =>
            await _accountQuerier.GetDestinationAccountListAsync(currentUserId, page, pageSize);

        public async Task<Result<AccountSummaryDTO>> OpenAccountAsync(string currentUserId, decimal balance)
        {
            using var trx = await UnitOfWorkScope.StartAsync(_unitOfWork);

            var result = await _accountService.OpenAccountAsync(currentUserId, balance);
            if (result.IsFailure)
                return result.Failure!;
            var (account, transaction) = result.Value;

            await _accountRepository.AddAsync(account);
            if (transaction is not null)
                await _transactionRepository.AddAsync(transaction);

            await trx.CommitAsync();
            return new AccountSummaryDTO(account.Id, account.Code, account.Balance);
        }

        public async Task<Result> CloseAccountAsync(string currentUserId, int accountId)
        {
            using var trx = await UnitOfWorkScope.StartAsync(_unitOfWork);
            Result<Account> getResult = await _accountRepository.GetAsync(currentUserId, accountId, true);
            if (getResult.IsFailure)
                return getResult;
            Account account = getResult.Value;

            Result closeResult = account.Close();
            if (closeResult.IsFailure)
                return closeResult;

            await _accountRepository.UpdateAsync(account);
            await trx.CommitAsync();
            return Result.Success();
        }

        public async Task<Result<WithdrawalTransactionDTO>> WithdrawAsync(string currentUserId, int accountId, decimal amount)
        {
            using var trx = await UnitOfWorkScope.StartAsync(_unitOfWork);

            Result<Account> getResult = await _accountRepository.GetAsync(currentUserId, accountId, true);
            if (getResult.IsFailure)
                return getResult.Failure!;
            Account account = getResult.Value;

            Result<WithdrawalTransaction> withdrawResult = await _accountService.WithdrawAsync(account, amount);
            if (withdrawResult.IsFailure)
                return withdrawResult.Failure!;
            WithdrawalTransaction transaction = withdrawResult.Value;

            await _transactionRepository.AddAsync(transaction);
            await _accountRepository.UpdateAsync(account);

            await trx.CommitAsync();
            return new WithdrawalTransactionDTO(transaction.CreatedAt, transaction.Amount, account.Code);
        }

        public async Task<Result<DepositTransactionDTO>> DepositAsync(string currentUserId, int accountId, decimal amount)
        {
            using var trx = await UnitOfWorkScope.StartAsync(_unitOfWork);

            Result<Account> getResult = await _accountRepository.GetAsync(currentUserId, accountId, true);
            if (getResult.IsFailure)
                return getResult.Failure!;
            Account account = getResult.Value;

            Result<DepositTransaction> depositResult = await _accountService.DepositAsync(account, amount);
            if (depositResult.IsFailure)
                return depositResult.Failure!;
            DepositTransaction transaction = depositResult.Value;

            await _transactionRepository.AddAsync(transaction);
            await _accountRepository.UpdateAsync(account);

            await trx.CommitAsync();
            return new DepositTransactionDTO(transaction.CreatedAt, transaction.Amount, account.Code);
        }

        public async Task<Result<TransferTransactionDTO>> TransferAsync(string currentUserId,
            int sourceAccountId, int destinationAccountId, decimal amount)
        {
            using var trx = await UnitOfWorkScope.StartAsync(_unitOfWork);

            Result<Account> getSourceResult = await _accountRepository.GetAsync(currentUserId, sourceAccountId, true);
            if (getSourceResult.IsFailure)
                return getSourceResult.Failure!;
            Account sourceAccount = getSourceResult.Value;

            Result<Account> getDestinationResult = await _accountRepository.GetAsync(destinationAccountId, true);
            if (getDestinationResult.IsFailure)
                return getDestinationResult.Failure!;
            Account destinationAccount = getDestinationResult.Value;
             
            Result<TransferTransaction> transferResult = await _accountService.TransferAsync(sourceAccount,
                destinationAccount, amount);
            if (transferResult.IsFailure)
                return transferResult.Failure!;
            TransferTransaction transaction = transferResult.Value;

            await _transactionRepository.AddAsync(transaction);
            await _accountRepository.UpdateAsync(sourceAccount);
            await _accountRepository.UpdateAsync(destinationAccount);

            await trx.CommitAsync();
            return new TransferTransactionDTO(transaction.CreatedAt, transaction.Amount,
                sourceAccount.Code, destinationAccount.Code);
        }
    }
}