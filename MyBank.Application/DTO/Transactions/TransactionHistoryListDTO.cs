namespace MyBank.Application.DTO.Transactions
{
    public record TransactionHistoryListDTO
    {
        public required List<TransactionHistoryItemDTO> Items { get; init; }
        public required int TotalCount { get; init; }
    }
}
