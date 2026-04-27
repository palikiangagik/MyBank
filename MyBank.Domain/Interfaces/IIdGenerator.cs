using MyBank.Domain.ValueObjects;

namespace MyBank.Domain.Interfaces
{
    public interface IIdGenerator
    {
        public Task<IntId> GetNextIdAsync();
    }
}
