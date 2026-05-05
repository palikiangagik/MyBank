namespace MyBank.Application.DTO.Accounts
{
    public record DestinationAccountListDTO
    {
        public required int TotalCount { get; init; }
        public required List<DestinationAccountDTO> Items { get; init; }
    }
}
