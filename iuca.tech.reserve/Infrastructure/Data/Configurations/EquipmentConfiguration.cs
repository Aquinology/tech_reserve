using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.Property(x => x.SerialNumber)
            .HasMaxLength(32)
            .IsRequired(false);

        builder.Property(x => x.ImgLink)
            .HasMaxLength(256)
            .IsRequired(false);
    }
}
