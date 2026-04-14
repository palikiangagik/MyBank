namespace MyBank.Portal.Contracts.Account
{
    public record AccountListItemDTO
    {
        public int Id { get; init; }
        public decimal Balance { get; init; }
    }
}