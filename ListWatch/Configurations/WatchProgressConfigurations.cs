using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ListWatch.Models;
namespace ListWatch.Configurations
{
    public class WatchProgressConfigurations : IEntityTypeConfiguration<WatchProgress>
    {
        public void Configure(EntityTypeBuilder<WatchProgress> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CompletedEpisodes)
                .IsRequired()
                .HasDefaultValue(0);
            builder.Property(x => x.TotalEpisodes)
                .IsRequired();
        }
    }
}
