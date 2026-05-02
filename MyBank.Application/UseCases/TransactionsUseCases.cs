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

        public async Task<SubList<TransactionHistoryItemDTO>> GetTransactionHistoryAsync(int clientId, 
            PagingParametersDTO pagingParameters) =>
            await _transactionQuerier.GetTransactionHistoryAsync(clientId, pagingParameters);
    }
}
