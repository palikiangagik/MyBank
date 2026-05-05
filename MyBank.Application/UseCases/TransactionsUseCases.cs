using MyBank.Application.DTO.Common;
using MyBank.Application.DTO.Transactions;
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

        public async Task<TransactionHistoryListDTO> GetTransactionHistoryAsync(int clientId, 
            PagingParametersDTO pagingParameters) =>
            await _transactionQuerier.GetTransactionHistoryAsync(clientId, pagingParameters);
    }
}
