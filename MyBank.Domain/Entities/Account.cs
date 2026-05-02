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
        public string Code { get; private set; }
        public IntId ClientId { get; }
        public Money Balance { get; private set; }
        public bool IsClosed { get; private set; }

        public Account(IntId id, string code, IntId clientId, Money balance, bool isClosed)
        {
            Id = id;
            Code = code;
            ClientId = clientId;
            Balance = balance;
            IsClosed = isClosed;
        }      

        internal Result Deposit(Money amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Deposit amount should be positive.");
            if (IsClosed)
                return Errors.OperationOnClosedAccount;

            Balance += amount;

            return Result.Success();
        }

        internal Result Withdraw(Money amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Withdrawal amount should be positive.");
            if (IsClosed)
                return Errors.OperationOnClosedAccount;
            if (amount > Balance)
                return Errors.InsufficientFunds;

            Balance -= amount;

            return Result.Success();
        }

        internal static Account Open(IntId id, IntId clientId)
        {
            string code = ((int)id).ToString("D6");
            return new Account(id, code, clientId, 0m, false);
        }

        public Result Close()
        {
            if (IsClosed)
                return Errors.AccountAlreadyClosed;
            if (Balance > 0)
                return Errors.CannotCloseAccountWithBalance;

            IsClosed = true;

            return Result.Success();
        }
    }
}
