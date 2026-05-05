using CorePrimitives;
using MyBank.Application.DTO.Accounts;
using MyBank.Application.DTO.Common;

namespace MyBank.Application.Interfaces
{
    public interface IAccountQuerier
    {
        public Task<Result<AccountSummaryDTO>> GetAccountSummaryAsync(int clientId, int accountId);
        public Task<AccountSummaryListDTO> GetClientAccountListAsync(int clientId, PagingParametersDTO pagingParameters);
        public Task<DestinationAccountListDTO> GetDestinationAccountListAsync(int clientId, PagingParametersDTO pagingParameters);
    }
}