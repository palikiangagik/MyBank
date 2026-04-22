using System.Collections.Generic;

namespace MyBank.Web.Contracts.Account.DTO
{
    public class TransactionListDTO
    {
        public List<TransactionListItemDTO> Items { get; init; }
        public int TotalCount { get; init; }
    }
}
