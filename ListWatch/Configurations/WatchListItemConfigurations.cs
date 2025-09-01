using ListWatch.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ListWatch.Data.Configurations
{
    public class WatchListItemsConfiguration : IEntityTypeConfiguration<WatchListItems>
    {
        public void Configure(EntityTypeBuilder<WatchListItems> builder)
        {
            builder.HasKey(w => w.Id);

            builder.Property(w => w.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(w => w.ItemType)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(w => w.Genre)
                   .HasMaxLength(100);

            builder.Property(w => w.ReleaseYear)
                   .IsRequired();

            builder.Property(w => w.Status)
                   .HasMaxLength(50);

            // FK configured in UserConfiguration, but we can also add clarity:
            builder.HasOne(w => w.User)
                   .WithMany(u => u.WatchListItems)
                   .HasForeignKey(w => w.UserId);

            builder.HasOne(x => x.WatchProgress)
                .WithOne(p => p.WatchListItems)
                .HasForeignKey<WatchProgress>(p => p.WatchListItemId);
        }
    }
}
