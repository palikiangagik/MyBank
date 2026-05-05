using MyBank.Application.DTO.Common;
using MyBank.Application.DTO.Transactions;

namespace MyBank.Application.Interfaces
{
    public interface ITransactionQuerier
    {
        public Task<TransactionHistoryListDTO> GetTransactionHistoryAsync(int clientId, PagingParametersDTO pagingParameters);
    }
}
