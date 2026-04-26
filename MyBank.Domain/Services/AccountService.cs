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

        public async Task<Account> OpenAccountAsync(string userId)
        {
            IntId id = await _accountRepository.GetNextIdAsync();
            return Account.Open(id, userId);
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
