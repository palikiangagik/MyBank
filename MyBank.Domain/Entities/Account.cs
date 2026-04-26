using CorePrimitives;
using MyBank.Domain.Common;
using MyBank.Domain.ValueObjects;

namespace MyBank.Domain.Entities
{
    // we use exceptions for exceptional contract/invariant viloation or non business logic failures
    // we use failures for business logic viloation

    public class Account
    {
        public IntId Id { get; private set; }
        public StringId Code { get; private set; }
        public StringId UserId { get; }
        public Money Balance { get; private set; }
        public bool IsClosed { get; private set; }

        // TODO: move params to record?
        public Account(IntId id, StringId code, StringId userId, Money balance, bool isClosed)
        {
            Id = id;
            Code = code;
            UserId = userId;
            Balance = balance;
            IsClosed = isClosed;
        }      

        internal Result Deposit(Money amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Deposit amount should be positive.");
            if (IsClosed)
                return Failures.OperationOnClosedAccount;

            Balance += amount;

            return Result.Success();
        }

        internal Result Withdraw(Money amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Withdrawal amount should be positive.");
            if (IsClosed)
                return Failures.OperationOnClosedAccount;
            if (amount > Balance)
                return Failures.InsufficientFunds;

            Balance -= amount;

            return Result.Success();
        }

        internal static Account Open(IntId id, StringId userId)
        {
            string code = ((int)id).ToString("D6");
            return new Account(id, code, userId, 0m, false);
        }

        public Result Close()
        {
            if (IsClosed)
                return Failures.AccountAlreadyClosed;
            if (Balance > 0)
                return Failures.CannotCloseAccountWithBalance;

            IsClosed = true;

            return Result.Success();
        }
    }
}
