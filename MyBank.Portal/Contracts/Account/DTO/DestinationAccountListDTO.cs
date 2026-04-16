using System.Collections.Generic;

namespace MyBank.Portal.Contracts.Account.DTO
{
    public record DestinationAccountListDTO
    {
        public List<DestinationAccountListItemDTO> Items { get; init; }
        public int TotalCount { get; init; }
    }
}
