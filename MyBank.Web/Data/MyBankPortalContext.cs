using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyBank.Web.Data.Models;
using System.Threading;

namespace MyBank.Web.Data
{
    public class MyBankWebContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Account> Accounts { get; set; }
 
        public MyBankWebContext(DbContextOptions<MyBankWebContext> options)
            : base(options)
        {     
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
    }
}
