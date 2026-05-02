using CorePrimitives;
using MyBank.Application.DTO;

namespace MyBank.Application.Interfaces
{
    public interface IAccountQuerier
    {
        public Task<Result<AccountSummaryDTO>> GetAccountSummaryAsync(int clientId, int accountId);
        public Task<SubList<AccountSummaryDTO>> GetClientAccountListAsync(int clientId, PagingParametersDTO pagingParameters);
        public Task<SubList<DestinationAccountDTO>> GetDestinationAccountListAsync(int clientId, PagingParametersDTO pagingParameters);
    }
}
