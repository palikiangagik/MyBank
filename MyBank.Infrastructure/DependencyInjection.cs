using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using MyBank.Application.Interfaces;
using MyBank.Domain.Interfaces; 
using MyBank.Infrastructure.Persistence;
using MyBank.Infrastructure.Persistence.Queries;
using MyBank.Infrastructure.Persistence.Repositories;
using MyBank.Infrastructure.Identity;

namespace MyBank.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMyBankInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddMemoryCache();

            // from domain layer
            services.AddScoped<IIdGenerator, IdGenerator>();

            // from app layer                        
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            
            services.AddScoped<IAccountQuerier, AccountQuerier>();
            services.AddScoped<IClientQuerier, ClientQuerier>();
            services.AddScoped<ITransactionQuerier, TransactionQuerier>();

            // for DB
            services.AddDbContext<MyBankDbContext>(options => options.UseSqlServer(connectionString));            
            services.AddScoped<IDbSession, DbSession>();
            services.AddScoped<DevelopmentDbSeeder>();

            return services;
        }

        public static IdentityBuilder AddMyBankIdentity(this IServiceCollection services)
        {
            services.AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies();

            services.AddScoped<ClientIdentityService>();

            return services.AddIdentityCore<IdentityUser>(options =>
            {
                options.Stores.MaxLengthForKeys = 128;
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<MyBankDbContext>()
            .AddClaimsPrincipalFactory<MyBankPrincipalFactory>();
        }

        public static async Task InitializeDatabaseAsync(this IServiceProvider services)
        {
            var dbContext = services.GetRequiredService<MyBankDbContext>();
            await dbContext.Database.MigrateAsync();
        }
    }
}
