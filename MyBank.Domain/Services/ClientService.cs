using CorePrimitives;
using MyBank.Domain.Entities;
using MyBank.Domain.Interfaces;
using MyBank.Domain.ValueObjects;

namespace MyBank.Domain.Services
{
    public class ClientService
    {
        private readonly IIdGenerator _idGenerator;

        public ClientService(IIdGenerator idGenerator)
        {
            _idGenerator = idGenerator;
        }

        public async Task<Client> RegisterClientAsync(ClientName name)
        {
            IntId clientId = await _idGenerator.GetNextIdAsync();
            return new Client(clientId, name);
        }
    }
}
