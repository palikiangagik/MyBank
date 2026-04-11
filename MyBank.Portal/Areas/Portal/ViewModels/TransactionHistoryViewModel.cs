using System;
using System.Collections.Generic;
using MyBank.Portal.Models;

namespace MyBank.Portal.Areas.Portal.ViewModels
{
    public class TransactionHistoryAccountViewItem
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    public class TransactionHistoryViewItem
    {
        public TransactionType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Amount { get; set; }
        public TransactionHistoryAccountViewItem Sender { get; set; }
        public TransactionHistoryAccountViewItem Recipient { get; set; }
    }

    public class TransactionHistoryViewModel
    {
        public List<TransactionHistoryViewItem> Transactions { get; set; }
    }
}
