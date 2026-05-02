using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MyBank.Application.UseCases;
using MyBank.Infrastructure.Persistence;
using System.Security.Claims;

namespace MyBank.Infrastructure.Identity
{
    public class MyBankPrincipalFactory : UserClaimsPrincipalFactory<IdentityUser>
    {
        private readonly IMemoryCache _cache;
        private readonly MyBankDbContext _db;
        private readonly ClientIdentityService _clientIdentityService;
        private readonly ClientUseCases _clientUseCases;

        public MyBankPrincipalFactory(            
            UserManager<IdentityUser> userManager,
            IOptions<IdentityOptions> optionsAccessor,
            IMemoryCache cache,
            MyBankDbContext db,
            ClientIdentityService clientIdentityService,
            ClientUseCases clientUseCases
        ) : base(userManager, optionsAccessor)
        {
            _cache = cache;
            _db = db;
            _clientIdentityService = clientIdentityService;
            _clientUseCases = clientUseCases;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(IdentityUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            // adding ClientId to the claims
            int clientId = await _cache.GetOrCreateAsync($"client-id:{user.Id}", async entry =>
            {
                var result = await _clientIdentityService.GetClientIdByUserIdAsync(user.Id);
                if (result.Failed)
                {
                    string err = $"Profile can not be found for IdentityUser: {user.Id}." +
                        "Ensure that ClientIdentityService.RegisterClientAsync() used for registration process.";
                    throw new InvalidOperationException(err);
                }                

                return result.Value;
            });
            identity.AddClaim(new Claim("ClientId", clientId.ToString()));

            // adding client name to the claims
            string? clientName = await _cache.GetOrCreateAsync($"client-name:{user.Id}", async entry =>
            {
                var result = await _clientUseCases.GetClientNameAsync(clientId);
                if (result.Failed)
                {
                    string err = $"Profile can not be found for IdentityUser: {user.Id}." +
                        "Ensure that ClientIdentityService.RegisterClientAsync() used for registration process.";
                    throw new InvalidOperationException(err);
                }
                
                return result.Value.FirstName + " " + result.Value.LastName;
            });
            identity.AddClaim(new Claim("ClientName", clientName??""));
                     
            return identity;
        }
    }
}
