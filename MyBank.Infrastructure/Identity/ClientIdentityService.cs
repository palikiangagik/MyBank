using CorePrimitives;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyBank.Application.Interfaces;
using MyBank.Domain.Common;
using MyBank.Domain.Entities;
using MyBank.Domain.Services;
using MyBank.Domain.ValueObjects;
using MyBank.Infrastructure.DTO;
using MyBank.Infrastructure.Persistence;

namespace MyBank.Infrastructure.Identity
{
    public class ClientIdentityService
    {
        private readonly MyBankDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ClientService _clientService;
        private readonly IClientRepository _clientRepository;

        public ClientIdentityService(MyBankDbContext db,
            UserManager<IdentityUser> userManager, ClientService clientService, IClientRepository clientRepository)
        {
            _db = db;
            _userManager = userManager;
            _clientService = clientService;
            _clientRepository = clientRepository;
        }

        public async Task<Result<RegisterClientResultDTO>> RegisterClientAsync(RegisterClientDTO dto)
        {
            await using var trx = await _db.Database.BeginTransactionAsync();

            // create identity user
            var user = new IdentityUser { UserName = dto.Email, Email = dto.Email };
            var identityResult = await _userManager.CreateAsync(user, dto.Password);
            if (!identityResult.Succeeded)
                return identityResult.Errors.Select(e => new Error(e.Code, e.Description)).ToList();

            // create client
            var name = new ClientName { FirstName = dto.FirstName, LastName = dto.LastName };
            Client client = await _clientService.RegisterClientAsync(name);
            await _clientRepository.AddAsync(client);

            // link identity user with client
            const string sql = @"
                INSERT INTO ClientIdentity (ClientId, UserId) 
                VALUES (@ClientId, @UserId)";
            await _db.Connection.ExecuteAsync(sql, new
            {
                ClientId = client.Id.Value,
                UserId = user.Id
            }, transaction: _db.Transaction);

            await trx.CommitAsync();
            return new RegisterClientResultDTO {
                ClientId = client.Id.Value, 
                User = user
            };
        }

        public async Task<Result<int>> GetClientIdByUserIdAsync(string userId)
        {
            const string sql = "SELECT ClientId FROM ClientIdentity WHERE UserId = @UserId";
            int? clientId = await _db.Connection.ExecuteScalarAsync<int?>(
                sql, 
                new { UserId = userId }, 
                transaction: _db.Transaction
            );
            if (clientId == null)
                return Errors.ClientNotFound;
            return clientId.Value;
        }
    }
}
