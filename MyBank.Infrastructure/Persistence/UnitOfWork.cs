using MyBank.Application.Interfaces;

namespace MyBank.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        public async Task<int> SaveChangesAsync()
        {
            return 0;
        }
    }
}
