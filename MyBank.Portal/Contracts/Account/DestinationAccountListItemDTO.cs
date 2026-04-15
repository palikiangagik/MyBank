namespace MyBank.Portal.Contracts.Account
{
    public record DestinationAccountListItemDTO
    {
        public int Id { get; init; }
        public string UserName { get; init; }
    }
}
