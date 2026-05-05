using MyBank.Domain.Entities;

namespace MyBank.Application.DTO.Transactions
{
    public record TransactionHistoryItemDTO
    {
        public required int Id { get; init; }
        public required TransactionType Type { get; init; }
        public required DateTime CreatedAt { get; init; }
        public required decimal Amount { get; init; }

        public required string? AccountCode { get; init; }

        public required TransferPartyDTO? Sender { get; init; }
        public required TransferPartyDTO? Recipient { get; init; }
    }
}
