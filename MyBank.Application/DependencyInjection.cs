using Microsoft.Extensions.DependencyInjection;
using MyBank.Domain.Services;
using MyBank.Application.UseCases;

namespace MyBank.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // from domain layer
            services.AddScoped<AccountService>();

            //  from app layer
            services.AddScoped<AccountsUseCases>(); 
            services.AddScoped<ProfileUseCases>();
            services.AddScoped<TransactionsUseCases>();
            return services;
        }
    }
}
