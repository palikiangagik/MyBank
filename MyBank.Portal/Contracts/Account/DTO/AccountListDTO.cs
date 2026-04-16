using System.Collections.Generic;

namespace MyBank.Portal.Contracts.Account.DTO
{
    public record AccountListDTO
    {
        public List<AccountListItemDTO> Items { get; init; }
        public int TotalCount { get; init; }
        public decimal TotalBalance { get; init; }
    }
}
