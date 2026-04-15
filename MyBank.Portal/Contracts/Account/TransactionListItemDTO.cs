using System;
using MyBank.Portal.Models;

namespace MyBank.Portal.Contracts.Account
{
    public record TransactionListItemDTO
    {
        public int Id { get; init; }
        public TransactionType Type { get; init; }
        public DateTime CreatedAt { get; init; }
        public decimal Amount { get; init; }
        
        public int? SenderAccountId { get; init; }
        public string SenderUserName { get; init; }
        
        public int? RecipientAccountId { get; init; }
        public string RecipientUserName { get; init; }        
    }
}
