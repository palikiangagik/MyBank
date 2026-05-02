using CorePrimitives;
using MyBank.Application.DTO;

namespace MyBank.Application.Interfaces
{
    public interface IClientQuerier
    {
        public Task<Result<ClientNameDTO>> GetClientNameAsync(int clientId);
        public Task<Result<ClientSummaryDTO>> GetClientSummaryAsync(int clientId, PagingParametersDTO pagingParameters);
    }
}
