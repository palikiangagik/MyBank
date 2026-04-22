using System;
using MyBank.Web.Data.Models;

namespace MyBank.Web.Contracts.Account.DTO
{
    public record TransactionListItemDTO
    {
        public int Id { get; init; }
        public TransactionType Type { get; init; }
        public DateTime CreatedAt { get; init; }
        public decimal Amount { get; init; }
        
        public string SenderAccountCode { get; init; }
        public string SenderUserName { get; init; }
        
        public string RecepientAccountCode { get; init; }
        public string RecipientUserName { get; init; }        
    }
}
