using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class EquipmentRequestConfiguration : IEntityTypeConfiguration<EquipmentRequest>
{
    public void Configure(EntityTypeBuilder<EquipmentRequest> builder)
    {
        builder.HasOne(x => x.Equipment)
            .WithMany(x => x.EquipmentRequests)
            .HasForeignKey(x => x.EquipmentId);

        builder.HasOne(x => x.Request)
            .WithMany(x => x.RequestEquipments)
            .HasForeignKey(x => x.RequestId);
    }
}
