using Dapper;
using MyBank.Domain.Interfaces;
using MyBank.Domain.ValueObjects;

namespace MyBank.Infrastructure.Persistence
{
    public class IdGenerator : IIdGenerator
    {
        private readonly UnitOfWork _uow;

        public IdGenerator(UnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IntId> GetNextIdAsync()
        {
            var con = await _uow.GetConnection();
            const string sql = "SELECT NEXT VALUE FOR dbo.IdSequence";
            var nextId = await con.ExecuteScalarAsync<int>(sql, transaction: _uow.Transaction);
            return nextId;
        }
    }
}
