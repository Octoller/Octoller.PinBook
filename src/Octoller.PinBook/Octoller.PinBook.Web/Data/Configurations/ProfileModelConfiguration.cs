using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Octoller.PinBook.Web.Data.Model;

namespace Octoller.PinBook.Web.Data.Configurations
{
    public class ProfileModelConfiguration : IEntityTypeConfiguration<Profile>
    {
        public void Configure(EntityTypeBuilder<Profile> builder)
        {
            builder.ToTable("Profiles");
            builder.HasKey(p => p.Id);
            builder.HasAlternateKey(p => p.UserId);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.Location).HasMaxLength(150);
            builder.Property(p => p.Site).HasMaxLength(150);
            builder.Property(p => p.About).HasMaxLength(1000).HasDefaultValue("<none>");

            builder.Property(p => p.CreatedAt).IsRequired();
            builder.Property(p => p.CreatedBy).IsRequired().HasMaxLength(100);
            builder.Property(p => p.UpdatedAt).IsRequired();
            builder.Property(p => p.UpdatedBy).IsRequired().HasMaxLength(100);

            builder.HasOne(p => p.User).WithOne(u => u.Profile);
            builder.HasMany(p => p.BookLists).WithOne(bl => bl.Profile);

        }
    }
}
