using CorePrimitives;
using MyBank.Application.DTO;

namespace MyBank.Application.Interfaces
{
    public interface ITransactionQuerier
    {
        public Task<SubList<TransactionHistoryItemDTO>> GetTransactionHistoryAsync(
            string currentUserId, int page, int pageSize);
    }
}
