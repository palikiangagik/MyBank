namespace MyBank.Web.Contracts.Account.DTO
{
    public record AccountListItemDTO
    {
        public int Id { get; init; }
        public string Code { get; init; }
        public decimal Balance { get; init; }
    }
}   