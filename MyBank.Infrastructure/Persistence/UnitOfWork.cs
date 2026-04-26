using Microsoft.Data.SqlClient;
using MyBank.Application.Interfaces;
using System.Data;
using System.Data.Common;

namespace MyBank.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly string _connectionString;
        private IDbConnection? _connection;
        private IDbTransaction? _transaction;

        public UnitOfWork(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection Connection
        {
            get
            {
                if (_connection is null)
                {
                    _connection = new SqlConnection(_connectionString);
                    _connection.Open();
                }
                return _connection;
            }
        }

        public IDbTransaction Transaction => _transaction ??= Connection.BeginTransaction();

        public async Task SaveChangesAsync()
        {
            if (_transaction is null) 
                return;

            try
            { 
                await ((SqlTransaction)_transaction).CommitAsync();
            }
            catch
            {
                await ((SqlTransaction)_transaction).RollbackAsync();
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null; 
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }
    }
}
