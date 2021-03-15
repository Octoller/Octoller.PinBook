using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Octoller.PinBook.Web.Data.Model;

namespace Octoller.PinBook.Web.Data.Configurations
{
    public class AccountModelConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts");
            builder.HasKey(a => a.Id);
            builder.HasAlternateKey(a => a.VkId);

            builder.Property(a => a.Id).ValueGeneratedOnAdd();
            builder.Property(a => a.Name).IsRequired().HasMaxLength(500);
            builder.Property(a => a.Photo).HasColumnType("varbinary(max)");
            builder.Property(a => a.VkId).IsRequired().HasMaxLength(100);
            builder.Property(a => a.AccessToken).HasMaxLength(250);
            builder.Property(a => a.CreatedAt).IsRequired();
            builder.Property(a => a.CreatedBy).IsRequired().HasMaxLength(100);
            builder.Property(a => a.UpdatedAt);
            builder.Property(a => a.UpdatedBy).HasMaxLength(100);

            builder.HasIndex(a => a.Name);

            builder.HasOne(a => a.User).WithOne(u => u.Account);
        }
    }
}
