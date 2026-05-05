namespace MyBank.Application.DTO.Accounts
{
    public record AccountSummaryListDTO
    {
        public required int TotalCount { get; init; }
        public required List<AccountSummaryDTO> Items { get; init; }
    }
}
 