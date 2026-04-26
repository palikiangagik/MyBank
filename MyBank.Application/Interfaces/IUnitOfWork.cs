using System.Data;

namespace MyBank.Application.Interfaces
{
    public interface IUnitOfWork
    {
        public IDbConnection Connection { get; }
        public IDbTransaction Transaction { get; }
        public Task SaveChangesAsync();
    }
}
