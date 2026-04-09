using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyBank.Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyBank.Portal.Data
{
    public class MyBankPortalContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Account> Accounts { get; set; }
 
        public MyBankPortalContext(DbContextOptions<MyBankPortalContext> options)
            : base(options)
        {
            EnsureDatabaseCreated();            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }


        private void EnsureDatabaseCreated()
        {
            // Retry logic to handle DB startup delay
            int maxRetries = 5;
            int delayInSeconds = 5;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    Database.EnsureCreated();
                    break;
                }
                catch (Microsoft.Data.SqlClient.SqlException)
                {
                    if (i == maxRetries)
                        throw;
                    Thread.Sleep(delayInSeconds * 1000);
                }
            }
        }
    }
}
