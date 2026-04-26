using MyBank.Web.ViewModels;
using System;
using System.Collections.Generic;
using MyBank.Domain.Entities;

namespace MyBank.Web.Areas.Web.ViewModels
{
    public class TransactionHistoryViewItem : BaseViewModel
    {
        public TransactionType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Amount { get; set; }
        public string AccountCode { get; set; }
        public string SenderAccountCode { get; set; }
        public string SenderName { get; set; }
        public string RecipientAccountCode { get; set; }
        public string RecipientName { get; set; }
    }

    public class TransactionHistoryViewModel
    {
        public List<TransactionHistoryViewItem> Transactions { get; set; }
        public PageViewModel PageViewModel { get; set; }
    }
}
