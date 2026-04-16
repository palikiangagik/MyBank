namespace MyBank.Portal.Contracts.Account.DTO
{
    public record DestinationAccountListItemDTO
    {
        public int Id { get; init; }
        public string UserName { get; init; }
        public string Code { get; init; }
    }
}
