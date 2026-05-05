namespace MyBank.Api.DTO.Accounts
{
    public record AccountSummaryDTO
    {
        public required int Id { get; init; }
        public required string Code { get; init; }
        public required decimal Balance { get; init; }
    }
}
