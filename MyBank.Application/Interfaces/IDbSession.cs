using System.Data;
using System.Data.Common;

namespace MyBank.Application.Interfaces
{
    public interface IDbSession
    {
        public Task BeginTransactionAsync();
        public Task RollbackAsync();
        public void Rollback();
        public Task CommitAsync();
    }    
}
