namespace MyBank.Application.DTO.Transactions
{
    public record TransferTransactionDTO
    {
        public required DateTime CreatedAt { get; init; }
        public required decimal Amount { get; init; }
        public required string SenderAccountCode { get; init; }
        public required string RecipientAccountCode { get; init; }
    }
}
