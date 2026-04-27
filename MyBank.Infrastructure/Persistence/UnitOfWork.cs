using CorePrimitives;
using Microsoft.Data.SqlClient;
using MyBank.Application.Interfaces;
using System.Data;
using System.Data.Common;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MyBank.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork, IDisposable, IAsyncDisposable
    {
        private DbConnection? _connection;
        public DbTransaction? Transaction { get; private set; }

        private readonly string _connectionString;

        public UnitOfWork(string connectionString) => _connectionString = connectionString;
        
        private async Task EnsureConnectionOpenAsync()
        {
            if (_connection is null)
            {
                _connection = new SqlConnection(_connectionString);
                await _connection.OpenAsync();
            }
        }

        public async Task<DbConnection> GetConnection()
        {
            await EnsureConnectionOpenAsync();
            return _connection!;
        }

        public async Task BeginTransactionAsync()
        {
            if (Transaction is not null)
                throw new InvalidOperationException("Transaction already started.");
            if (_connection is null)
                await EnsureConnectionOpenAsync();
            Transaction = await _connection!.BeginTransactionAsync();
        }

        public async Task RollbackAsync()
        {
            if (Transaction is null)
                throw new InvalidOperationException("No active transaction to rollback.");

            try
            {
                await Transaction.RollbackAsync();
            }
            finally
            {
                await Transaction.DisposeAsync();
                Transaction = null;
            }
        }

        public void Rollback()
        {
            if (Transaction is null)
                throw new InvalidOperationException("No active transaction to rollback.");

            try
            {
                Transaction.Rollback();
            }
            finally
            {
                Transaction.Dispose();
                Transaction = null;
            }
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
    }
}
