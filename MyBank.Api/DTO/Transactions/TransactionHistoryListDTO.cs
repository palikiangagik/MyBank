namespace MyBank.Api.DTO.Transactions
{
    public class TransactionHistoryListDTO
    {
        public required List<TransactionHistoryItemDTO> Items { get; init; }
        public required int TotalCount { get; init; }
    }        
}
