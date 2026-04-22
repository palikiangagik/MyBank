using System.Collections.Generic;

namespace MyBank.Web.Contracts.Account.DTO
{
    public record DestinationAccountListDTO
    {
        public List<DestinationAccountListItemDTO> Items { get; init; }
        public int TotalCount { get; init; }
    }
}
