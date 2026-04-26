using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MyBank.Application;
using MyBank.Application.Interfaces;
using MyBank.Application.UseCases;
using MyBank.Domain.Interfaces;
using MyBank.Infrastructure.Persistence;
using MyBank.Infrastructure.Persistence.Queries;
using MyBank.Infrastructure.Persistence.Repositories;
using System.Timers;

namespace MyBank.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            // from domain layer 
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            
            // from app layer            
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAccountQuerier, AccountQuerier>();
            services.AddScoped<IProfileQuerier, ProfileQuerier>();
            services.AddScoped<ITransactionQuerier, TransactionQuerier>();                

            return services;
        }

        public static IdentityBuilder AddIdentityInfrastructure(this IServiceCollection services, string connectionString)
        {
            // identity storage
            services.AddDbContext<MyBankIdentityDbContext>(options =>
                options.UseSqlServer(
                    connectionString,
                    sqlOptions => sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 10,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null
                    )
                )
            );

            // identity services
            return services.AddIdentityCore<IdentityUser>(options =>
            {
                options.Stores.MaxLengthForKeys = 128;
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.SignIn.RequireConfirmedEmail = false;
            }).AddEntityFrameworkStores<MyBankIdentityDbContext>();
        }

    }
}
