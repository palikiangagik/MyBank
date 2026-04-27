using CorePrimitives;
using MyBank.Domain.Entities;
using MyBank.Domain.Interfaces; 
using MyBank.Domain.ValueObjects;


namespace MyBank.Domain.Services
{
    // consistency is maintained at the app level by IDbSession

    public class AccountService
    {
        private readonly IIdGenerator _idGenerator;        

        public AccountService(IIdGenerator idGenerator) 
        {
            _idGenerator = idGenerator;
        }

        public async Task<Result<(Account, DepositTransaction?)>> OpenAccountAsync(string userId, Money balance)
        {
            IntId accountId = await _idGenerator.GetNextIdAsync();

            Account account = Account.Open(accountId, userId);
            DepositTransaction? transaction = null;

            if (balance > 0)
            {
                IntId transactionId = await _idGenerator.GetNextIdAsync();

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
            IntId id = await _idGenerator.GetNextIdAsync();
            return account.Withdraw(amount)
                .Then(() => WithdrawalTransaction.Create(id, amount, account.Id));
        }

        public async Task<Result<DepositTransaction>> DepositAsync(Account account, Money amount)
        {
            IntId id = await _idGenerator.GetNextIdAsync();
            return account.Deposit(amount)
                .Then(() => DepositTransaction.Create(id, amount, account.Id));
        }

        public async Task<Result<TransferTransaction>> TransferAsync(Account sender, Account recipient, Money amount)
        {
            IntId id = await _idGenerator.GetNextIdAsync();
            return sender.Withdraw(amount)
                .Then(() => recipient.Deposit(amount))
                .Then(() => TransferTransaction.Create(id, amount, sender.Id, recipient.Id));
        }
    }
}
