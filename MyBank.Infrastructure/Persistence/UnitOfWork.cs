using Microsoft.Data.SqlClient;
using MyBank.Application.Interfaces;
using System.Data;
using System.Data.Common;

namespace MyBank.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly string _connectionString;
        private SqlConnection? _connection;
        private SqlTransaction? _transaction;

        public UnitOfWork(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlConnection Connection
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

        public SqlTransaction Transaction => _transaction ??= Connection.BeginTransaction();

        public async Task SaveChangesAsync()
        {
            if (_transaction is null) 
                return;

            try
            { 
                await _transaction.CommitAsync();
            }
            catch
            {
                await _transaction.RollbackAsync();
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
