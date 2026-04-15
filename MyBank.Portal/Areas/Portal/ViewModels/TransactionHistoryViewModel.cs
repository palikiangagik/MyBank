using System;
using System.Collections.Generic;
using MyBank.Portal.Data.Models;

namespace MyBank.Portal.Areas.Portal.ViewModels
{
    public class TransactionHistoryViewItem
    {
        public TransactionType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Amount { get; set; }
        public int? SenderId { get; set; }
        public string SenderName { get; set; }
        public int? RecipientId { get; set; }
        public string RecipientName { get; set; }
    }

    public class TransactionHistoryViewModel
    {
        public List<TransactionHistoryViewItem> Transactions { get; set; }
    }
}
