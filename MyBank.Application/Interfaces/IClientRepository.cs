using MyBank.Domain.Entities;

namespace MyBank.Application.Interfaces
{
    public interface IClientRepository
    {
        public Task AddAsync(Client client);
    }
}
