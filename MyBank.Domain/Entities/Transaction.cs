using CorePrimitives;
using MyBank.Domain.Common;
using MyBank.Domain.ValueObjects;

namespace MyBank.Domain.Entities
{
    // we use exceptions for exceptional contract viloation or non business logic failures
    // we use failures for business logic viloation

    public enum TransactionType
    {
        Transfer,
        Withdrawal,
        Deposit
    }

    public abstract class Transaction
    {
        public IntId Id { get; private set; }
        public DateTime CreatedAt { get; }
        public TransactionType Type { get; }
        public Money Amount { get; }

        protected Transaction(IntId id, DateTime createdAt, TransactionType type, Money amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive.");

            Id = id;
            CreatedAt = createdAt;
            Amount = amount;
            Type = type;
        }
    }

    public class TransferTransaction : Transaction
    {
        public IntId SenderAccountId { get; }
        public IntId RecipientAccountId { get; }

        public TransferTransaction(IntId id, DateTime createdAt, Money amount,
            IntId senderAccountId, IntId recipientAccountId) : base(id, createdAt, TransactionType.Transfer, amount)
        {
            if (recipientAccountId == senderAccountId)
                throw new ArgumentException("Recipient and sender can't be the same.");

            SenderAccountId = senderAccountId;
            RecipientAccountId = recipientAccountId;
        }

        internal static Result<TransferTransaction> Create(IntId id, Money amount,
            IntId senderAccountId, IntId recipientAccountId)
        {
            return new TransferTransaction(id, DateTime.UtcNow, amount, senderAccountId, recipientAccountId);
        }
    }

    public class WithdrawalTransaction : Transaction
    {
        public IntId AccountId { get; }

        public WithdrawalTransaction(IntId id, DateTime createdAt, Money amount,
            IntId accountId) : base(id, createdAt, TransactionType.Withdrawal, amount)
        {
            AccountId = accountId;
        }

        internal static Result<WithdrawalTransaction> Create(IntId id, Money amount, IntId accountId)
        {
            return new WithdrawalTransaction(id, DateTime.UtcNow, amount, accountId);
        }
    }

    public class DepositTransaction : Transaction
    {
        public IntId AccountId { get; }

        public DepositTransaction(IntId id, DateTime createdAt, Money amount,
            IntId accountId) : base(id, createdAt, TransactionType.Deposit, amount)
        {
            AccountId = accountId;
        }

        internal static Result<DepositTransaction> Create(IntId id, Money amount, IntId accountId)
        {
            return new DepositTransaction(id, DateTime.UtcNow, amount, accountId);
        }
    }
}
