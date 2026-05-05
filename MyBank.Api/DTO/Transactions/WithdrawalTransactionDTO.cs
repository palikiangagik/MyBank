namespace MyBank.Api.DTO.Transactions
{
    public record WithdrawalTransactionDTO
    {
        public required DateTime CreatedAt { get; init; }
        public required decimal Amount { get; init; }
        public required string AccountCode { get; init; }
    }
}
