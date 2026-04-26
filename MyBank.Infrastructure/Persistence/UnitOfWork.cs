using Microsoft.Data.SqlClient;
using MyBank.Application.Interfaces;
using System.Data;
using System.Data.Common;

namespace MyBank.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork, IDisposable, IAsyncDisposable
    {
        private DbConnection? _connection;
        public DbTransaction? Transaction { get; private set; }
        public DbTransaction? Trx => Transaction;

        private readonly string _connectionString;

        public UnitOfWork(string connectionString) => _connectionString = connectionString;

        public async Task<DbConnection> GetConnection()
        {
            await EnsureConnectionOpenAsync();
            return _connection!;
        }

        public async Task BeginTransactionAsync()
        {
            if (Transaction is not null) 
                return;
            if (_connection is null)
                await EnsureConnectionOpenAsync();
            Transaction = await _connection!.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (Transaction is null)
                throw new InvalidOperationException("No active transaction to commit.");    

            try
            { 
                await Transaction.CommitAsync();
            }
            catch
            {
                await Transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await Transaction.DisposeAsync();
                Transaction = null; 
            }
        }

        public void Dispose()
        {
            if (Transaction is not null)
            {
                Transaction.Dispose();
                Transaction = null;
            }

            if (_connection is not null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (Transaction is not null)
            {
                await Transaction.DisposeAsync();
                Transaction = null;
            }

            if (_connection is not null)
            {
                await _connection.DisposeAsync();
                _connection = null;
            }
        }

        private async Task EnsureConnectionOpenAsync()
        {
            if (_connection is null)
            {
                _connection = new SqlConnection(_connectionString);
                await _connection.OpenAsync();
            }
        }
    }
}
