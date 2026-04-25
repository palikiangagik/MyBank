using CorePrimitives;
using MyBank.Application.DTO;
using MyBank.Application.Interfaces;

namespace MyBank.Infrastructure.Persistence.Queries
{
    public class TransactionQuerier : ITransactionQuerier
    {
        public async Task<Result<SubList<TransactionHistoryItemDTO>>> GetTransactionHistoryAsync(
           string currentUserId, int page, int pageSize)
        {
            return new Failure("NotImplemented", "NotImplemented");
        }
    }
}
