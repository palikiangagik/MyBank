namespace MyBank.Api.DTO
{
    public record ClientAccountsDTO
    {
        public required List<Api.DTO.AccountSummaryDTO> Accounts { get; init; } = [];
        public required int TotalCount { get; init; } = 0;
    }
}
