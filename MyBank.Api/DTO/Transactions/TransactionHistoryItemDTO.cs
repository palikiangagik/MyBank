namespace MyBank.Api.DTO.Transactions
{
    public record TransactionHistoryItemDTO
    {
        public required int Id { get; init; }
        public required string Type { get; init; }
        public required DateTime CreatedAt { get; init; }
        public required decimal Amount { get; init; }

        public required string? AccountCode { get; init; }

        public required TransferPartyDTO? Sender { get; init; }
        public required TransferPartyDTO? Recipient { get; init; }
    }
}
