using CorePrimitives;
using MyBank.Domain.ValueObjects;
using MyBank.Domain.Entities;

namespace MyBank.Application.Interfaces
{
    public interface IAccountRepository
    {
        public Task AddAsync(Account account);
        public Task<Result<Account>> GetAsync(IntId clientId, IntId accountId, bool blockUntilUpdate);
        public Task<Result<Account>> GetAsync(IntId accountId, bool blockUntilUpdate);
        public Task UpdateAsync(Account account);
    }
}
