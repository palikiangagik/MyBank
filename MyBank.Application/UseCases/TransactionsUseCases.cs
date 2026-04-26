using CorePrimitives;
using MyBank.Application.DTO;
using MyBank.Application.Interfaces;

namespace MyBank.Application.UseCases
{
    public class TransactionsUseCases
    {
        private readonly ITransactionQuerier _transactionQuerier;

        public TransactionsUseCases(ITransactionQuerier transactionQuerier)
        {
            _transactionQuerier = transactionQuerier;
        }

        public async Task<SubList<TransactionHistoryItemDTO>> GetTransactionHistoryAsync(
            string currentUserId, int page, int pageSize) =>
            await _transactionQuerier.GetTransactionHistoryAsync(currentUserId, page, pageSize);
    }
}
