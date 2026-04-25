using Microsoft.Extensions.DependencyInjection;

using MyBank.Application;
using MyBank.Application.Interfaces;
using MyBank.Application.UseCases;
using MyBank.Domain.Interfaces;
using MyBank.Infrastructure.Persistence;
using MyBank.Infrastructure.Persistence.Queries;
using MyBank.Infrastructure.Persistence.Repositories;

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
    }
}
