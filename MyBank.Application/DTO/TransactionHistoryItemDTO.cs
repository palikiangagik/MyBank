using MyBank.Domain.Entities;

namespace MyBank.Application.DTO
{
    public record TransactionHistoryItemDTO
    {
        public int Id { get; init; }
        public TransactionType Type { get; init; }
        public DateTime CreatedAt { get; init; }
        public decimal Amount { get; init; }

        public string? SenderAccountCode { get; init; }
        public string? SenderUserName { get; init; }

        public string? RecepientAccountCode { get; init; }
        public string? RecipientUserName { get; init; }
    }
}
