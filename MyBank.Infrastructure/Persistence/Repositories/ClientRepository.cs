using Dapper;
using Microsoft.EntityFrameworkCore;
using MyBank.Application.Interfaces;
using MyBank.Domain.Entities;

namespace MyBank.Infrastructure.Persistence.Repositories
{
    public class ClientRepository: IClientRepository
    {
        private readonly MyBankDbContext _db;

        public ClientRepository(MyBankDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Client client)
        {
            const string sql = @"
                INSERT INTO Clients (Id, FirstName, LastName) 
                VALUES (@Id, @FirstName, @LastName)";

            await _db.Connection.ExecuteAsync(sql, new
            {
                Id = client.Id.Value,
                FirstName = client.Name.FirstName,
                LastName = client.Name.LastName
            }, transaction: _db.Transaction);
        }
    }
}
