using BankingApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Configurations
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<AccountEntity> Accounts => Set<AccountEntity>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
        }
    }
}