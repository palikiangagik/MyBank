namespace MyBank.Api.DTO
{
    public record DepositTransactionDTO
    {
        public required DateTime CreatedAt { get; init; }
        public required decimal Amount { get; init; }
        public required string AccountCode { get; init; }
    }
}
