using MyBank.Application.Interfaces;

namespace MyBank.Application.Helpers
{
    public class TransactionScope : IAsyncDisposable
    {
        private readonly IDbSession _db;
        private bool _commited;

        private TransactionScope(IDbSession db)
        {
            _db = db;
            _commited = false;
        }

        public static async Task<TransactionScope> StartAsync(IDbSession db)
        {
            await db.BeginTransactionAsync();
            return new TransactionScope(db);
        }

        public async Task CommitAsync()
        {
            await _db.CommitAsync();
            _commited = true;
        }

        public async ValueTask DisposeAsync()
        {
            if (!_commited)
                await _db.RollbackAsync();
        }
    }
}
