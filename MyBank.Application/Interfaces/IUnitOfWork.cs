using System.Data;
using System.Data.Common;

namespace MyBank.Application.Interfaces
{
    public interface IUnitOfWork
    {
        public Task BeginTransactionAsync();
        public Task CommitAsync();
    }
}
