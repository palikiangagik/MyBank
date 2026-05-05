using CorePrimitives;
using MyBank.Application.Interfaces;
using MyBank.Application.DTO.Client;
using MyBank.Application.DTO.Common;

namespace MyBank.Application.UseCases
{
    public class ClientUseCases
    {
        private readonly IClientQuerier _clientQuerier;

        public ClientUseCases(IClientQuerier clientQuerier)
        {
            _clientQuerier = clientQuerier;
        }

        public Task<Result<ClientNameDTO>> GetClientNameAsync(int clientId) =>
            _clientQuerier.GetClientNameAsync(clientId);

        public Task<Result<ClientSummaryDTO>> GetClientSummaryAsync(int clientId, PagingParametersDTO pagingParameters) =>
            _clientQuerier.GetClientSummaryAsync(clientId, pagingParameters);
    }
}
