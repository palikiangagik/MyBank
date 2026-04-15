using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyBank.Portal.Models;
using System.Threading;

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

            builder.Entity<Transaction>()
                .HasOne(t => t.Sender)
                .WithMany() 
                .HasForeignKey(t => t.SenderId)
                .OnDelete(DeleteBehavior.Restrict); 

            builder.Entity<Transaction>()
                .HasOne(t => t.Recipient)
                .WithMany()
                .HasForeignKey(t => t.RecipientId)
                .OnDelete(DeleteBehavior.Restrict); 
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
                    if (i == maxRetries - 1)
                        throw;
                    Thread.Sleep(delayInSeconds * 1000);
                }
            }
        }
    }
}
