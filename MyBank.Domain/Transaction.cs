using CorePrimitives;

namespace MyBank.Domain
{
    // we use exceptions for exceptional contract viloation or non business logic failures
    // we use failures for business logic viloation

    public enum TransactionType
    {
        Transfer,
        Withdrawal,
        Deposit
    }

    public record TransactionParams(TransactionType Type, decimal Amount,
            int? SenderAccountId, int? RecipientAccountId);

    public class Transaction
    {
        public int? Id { get; private set; }
        public DateTime CreatedAt { get; }
        public TransactionType Type { get; }
        public decimal Amount { get; }
        public int? SenderAccountId { get; }
        public int? RecipientAccountId { get; }

        // too much params, but it will be called by dapper only, so we keep it to keep the dapper mapping simpler
        public Transaction(int id, DateTime createdAt, TransactionType type,
            decimal amount, int? senderAccountId, int? recipientAccountId) : 
            this(createdAt, new TransactionParams(type, amount, senderAccountId, recipientAccountId))
        {
            SetId(id);
        }

        // common code for public ctor and Create()
        private Transaction(DateTime createdAt, TransactionParams transactionParams)
        {
            if ((transactionParams.RecipientAccountId is null) && (transactionParams.SenderAccountId is null))
                throw new ArgumentException("Either recipient or sender should be set.");
            if (transactionParams.RecipientAccountId  == transactionParams.SenderAccountId)
                throw new ArgumentException("Recipient and sender can't be the same.");
            if (transactionParams.Amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(transactionParams.Amount), "Amount must be positive.");

            switch(transactionParams.Type)
            {
                case TransactionType.Transfer:                
                    if (transactionParams.SenderAccountId is null || transactionParams.RecipientAccountId is null)
                        throw new ArgumentException("For transfer both sender and recipient should be set.");
                    break;
                case TransactionType.Withdrawal:
                    if (transactionParams.SenderAccountId is null || transactionParams.RecipientAccountId is not null)
                        throw new ArgumentException("For withdrawal only the sender should be set and recipient shouldn't be set.");
                    break;
                case TransactionType.Deposit:
                    if (transactionParams.SenderAccountId is not null || transactionParams.RecipientAccountId is null)
                        throw new ArgumentException("For deposit only the recipient should be set and sender shouldn't be set.");
                    break;
            }

            Id = null;
            CreatedAt = createdAt;
            Amount = transactionParams.Amount;
            Type = transactionParams.Type;
            SenderAccountId = transactionParams.SenderAccountId;
            RecipientAccountId = transactionParams.RecipientAccountId;
        }

        public void SetId(int id)
        {
            if (Id is not null)
                throw new InvalidOperationException("Id already set.");
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "Id must be positive");

            Id = id;
        }

        public static Transaction Create(TransactionParams transactionsParams)
        {
            return new Transaction(DateTime.UtcNow, transactionsParams);
        }
    }
}
