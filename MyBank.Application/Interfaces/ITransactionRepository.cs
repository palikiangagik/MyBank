using MyBank.Domain.Entities;
using MyBank.Domain.ValueObjects;

namespace MyBank.Application.Interfaces
{
    public interface ITransactionRepository
    {
        public Task AddAsync(Transaction transaction);
    }
}
