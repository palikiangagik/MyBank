using MyBank.Domain.Entities;

namespace MyBank.Api.DTO
{
    public class TransactionHistoryDTO
    {
        public record Item
        {
            public required string Type { get; set; }
            public DateTime CreatedAt { get; set; }
            public decimal Amount { get; set; }
            public string? AccountCode { get; set; }
            public string? SenderAccountCode { get; set; }
            public string? SenderName { get; set; }
            public string? RecipientAccountCode { get; set; }
            public string? RecipientName { get; set; }
        }

        public List<Item> Items { get; set; } = [];
        public int TotalCount { get; set; }
    }

        
}
