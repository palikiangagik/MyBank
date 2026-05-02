using Dapper;
using MyBank.Domain.Interfaces;
using MyBank.Domain.ValueObjects;

namespace MyBank.Infrastructure.Persistence
{
    public class IdGenerator : IIdGenerator
    {
        private readonly MyBankDbContext _db;

        public IdGenerator(MyBankDbContext db)
        {
            _db = db;
        }

        public async Task<IntId> GetNextIdAsync()
        {
            const string sql = "SELECT NEXT VALUE FOR IdSequence";
            var nextId = await _db.Connection.ExecuteScalarAsync<int>(sql, transaction: _db.Transaction);
            return nextId;
        }
    }
}
