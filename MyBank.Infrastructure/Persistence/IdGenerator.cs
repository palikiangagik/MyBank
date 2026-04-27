using Dapper;
using MyBank.Domain.Interfaces;
using MyBank.Domain.ValueObjects;

namespace MyBank.Infrastructure.Persistence
{
    public class IdGenerator : IIdGenerator
    {
        private readonly DbSession _db;

        public IdGenerator(DbSession db)
        {
            _db = db;
        }

        public async Task<IntId> GetNextIdAsync()
        {
            var con = await _db.GetConnection();
            const string sql = "SELECT NEXT VALUE FOR dbo.IdSequence";
            var nextId = await con.ExecuteScalarAsync<int>(sql, transaction: _db.Transaction);
            return nextId;
        }
    }
}
