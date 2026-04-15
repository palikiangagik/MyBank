using System;

namespace MyBank.Portal.Data.Models
{
    public enum TransactionType
    {
        Transfer,
        Withdrawal,
        Deposit
    }

    public class Transaction
    {
        public int Id { get; set; }
        public TransactionType Type { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public decimal Amount { get; set; } 
        
        public int? RecipientId { get; set; }
        public Account Recipient { get; set; }

        public int? SenderId { get; set; }   
        public Account Sender { get; set; }
    }
}
