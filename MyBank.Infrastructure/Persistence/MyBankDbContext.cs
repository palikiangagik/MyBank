using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;

namespace MyBank.Infrastructure.Persistence
{
    public class MyBankDbContext : IdentityDbContext<IdentityUser>
    {
        public DbConnection Connection => Database.GetDbConnection();
        public DbTransaction? Transaction => Database.CurrentTransaction?.GetDbTransaction();

        public MyBankDbContext(DbContextOptions<MyBankDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }        
    }
}
