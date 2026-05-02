using MyBank.Domain.Entities;

namespace MyBank.Application.DTO
{
    public record TransactionHistoryItemDTO
    {
        public record Party
        {
            public required string FirstName { get; init; }
            public required string LastName { get; init; }
            public required string AccountCode { get; init; }
        }

        public int Id { get; init; }
        public TransactionType Type { get; init; }
        public DateTime CreatedAt { get; init; }
        public decimal Amount { get; init; }

        public string? AccountCode { get; init; }

        public Party? Sender { get; init; }
        public Party? Recipient { get; init; }
    }
}
