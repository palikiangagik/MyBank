using CorePrimitives;
using MyBank.Domain.Entities;
using MyBank.Domain.Interfaces;
using MyBank.Domain.ValueObjects;


namespace MyBank.Domain.Services
{
    // consistency is maintained at the app level by UoW

    public class AccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;

        public AccountService(IAccountRepository accountRepository,
            ITransactionRepository transactionRepository)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<Result<(Account, DepositTransaction?)>> OpenAccountAsync(string userId, Money balance)
        {
            IntId accountId = await _accountRepository.GetNextIdAsync();

            Account account = Account.Open(accountId, userId);
            DepositTransaction? transaction = null;

            if (balance > 0)
            {
                IntId transactionId = await _transactionRepository.GetNextIdAsync();

                var transactionResult = account.Deposit(balance)
                    .Then(() => DepositTransaction.Create(transactionId, balance, account.Id));
                if (transactionResult.IsFailure)
                    return transactionResult.Failure!;
                transaction = transactionResult.Value;
            }

            return (account, transaction);
        }

        public async Task<Result<WithdrawalTransaction>> WithdrawAsync(Account account, Money amount)
        {
            IntId id = await _transactionRepository.GetNextIdAsync();
            return account.Withdraw(amount)
                .Then(() => WithdrawalTransaction.Create(id, amount, account.Id));
        }

        public async Task<Result<DepositTransaction>> DepositAsync(Account account, Money amount)
        {
            IntId id = await _transactionRepository.GetNextIdAsync();
            return account.Deposit(amount)
                .Then(() => DepositTransaction.Create(id, amount, account.Id));
        }

        public async Task<Result<TransferTransaction>> TransferAsync(Account sender, Account recipient, Money amount)
        {
            IntId id = await _transactionRepository.GetNextIdAsync();
            return sender.Withdraw(amount)
                .Then(() => recipient.Deposit(amount))
                .Then(() => TransferTransaction.Create(id, amount, sender.Id, recipient.Id));
        }
    }
}
