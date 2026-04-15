using System.Collections.Generic;

namespace MyBank.Portal.Contracts.Account
{
    public class TransactionListDTO
    {
        public List<TransactionListItemDTO> Items { get; init; }
        public int TotalCount { get; init; }
    }
}
