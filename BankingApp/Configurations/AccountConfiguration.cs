using BankingApp.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingApp.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<AccountEntity>
    {
        public void Configure(EntityTypeBuilder<AccountEntity> builder)
        {
            builder.HasKey(i => i.Id);

            builder.HasIndex(i => i.AccountNumber)
                .IsUnique();

            builder.Property(i => i.Balance)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        }
    }
}