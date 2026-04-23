using CorePrimitives;

namespace MyBank.Domain
{
    // we use exceptions for exceptional contract viloation or non business logic failures
    // we use failures for business logic viloation
        
    public class Account
    {
        public int? Id { get; private set; }
        public string? Code { get; private set; }
        public string UserId { get; }
        public decimal Balance { get; private set; } 
        public bool IsClosed { get; private set; } 


        // too much params, but it will be called by dapper only, so we keep it to keep the dapper mapping simpler
        public Account(int id,  string code, string userId, decimal balance, bool isClosed) :
            this(userId, balance, isClosed)
        {
            SetId(id);
            SetCode(code);
        }


        // common code for public ctor and Open()
        private Account(string userId, decimal balance, bool isClosed) 
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));
            if (balance < 0)
                throw new ArgumentOutOfRangeException(nameof(balance), "Balance can't be negative.");

            Id = null;
            Code = null;
            UserId = userId;
            Balance = balance;
            IsClosed = isClosed;
        }

        public void SetId(int id)
        {
            if (Id is not null)
                throw new InvalidOperationException("Id already set.");
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "Id must be positive");

            Id = id;
        }

        public void SetCode(string code)
        {
            if (Code is not null)
                throw new InvalidOperationException("Code already set.");
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Code cannot be empty.", nameof(code));

            Code = code;
        }

        public Result Deposit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Deposit amount should be positive.");
            if (IsClosed)
                return Failures.OperationOnClosedAccount;

            Balance += amount;

            return Result.Success();
        }

        public Result Withdraw(decimal amount)
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


        public Result Close()
        {
            if (IsClosed)
                return Failures.AccountAlreadyClosed;
            if (Balance > 0)
                return Failures.CannotCloseAccountWithBalance;

            IsClosed = true;

            return Result.Success();
        }

        public static Account Open(string userId)
        {
            return new Account(userId, 0m, false);
        }
    }
}
