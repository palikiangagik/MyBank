using CorePrimitives;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MyBank.Application.UseCases;
using MyBank.Domain.Entities;
using MyBank.Infrastructure.Persistence;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace MyBank.Infrastructure.Identity
{
    public class MyBankPrincipalFactory : UserClaimsPrincipalFactory<IdentityUser>
    {
        private readonly IMemoryCache _cache;
        private readonly MyBankDbContext _db;

        public MyBankPrincipalFactory(            
            UserManager<IdentityUser> userManager,
            IOptions<IdentityOptions> optionsAccessor,
            IMemoryCache cache,
            MyBankDbContext db
        ) : base(userManager, optionsAccessor)
        {
            _cache = cache;
            _db = db;
        }

        private record Client(int Id, string Name);

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(IdentityUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            Client? client = await GetClient(user) ??
                throw new InvalidOperationException($"Client can not be found for IdentityUser: {user.Id}." +
                    "Ensure that ClientIdentityService.RegisterClientAsync() used for registration process.");

            identity.AddClaim(new Claim("ClientId", client.Id.ToString()));           
            identity.AddClaim(new Claim("ClientName", client.Name));
                     
            return identity;
        }      

        private async Task<Client?> GetClient(IdentityUser user)
        {
            return await _cache.GetOrCreateAsync($"client-id:{user.Id}", async entry =>
            {
                const string sql = @"
                    SELECT 
                        CI.ClientId AS Id,
                        C.FirstName,
                        C.LastName
                    FROM 
                        ClientIdentity as CI
                        LEFT JOIN Clients as C ON CI.ClientId = C.Id
                    WHERE 
                        CI.UserId = @UserId
                ";
                var row = await _db.Connection.QueryFirstOrDefaultAsync(sql, new { UserId = user.Id });
                if (row is null)
                    return null;
                return new Client(row.Id, row.FirstName + " " + row.LastName);
            });
        }
    }
}
