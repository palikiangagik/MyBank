using CorePrimitives;
using MyBank.Domain.ValueObjects;
using MyBank.Domain.Entities;

namespace MyBank.Application.Interfaces
{
    public interface IAccountRepository
    {
        public Task AddAsync(Account account);
        public Task<Result<Account>> GetAsync(StringId userId, IntId accountId, bool blockUntilUpdat);
        public Task<Result<Account>> GetAsync(IntId accountId, bool blockUntilUpdat);
        public Task UpdateAsync(Account account);
    }
}
