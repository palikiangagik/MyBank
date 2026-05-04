namespace MyBank.Api.DTO
{
    public record ClientDestinationAccountsDTO
    {
        public required List<Api.DTO.DestinationAccountDTO> Accounts { get; init; } = [];
        public required int TotalCount { get; init; } = 0;
    }
}
