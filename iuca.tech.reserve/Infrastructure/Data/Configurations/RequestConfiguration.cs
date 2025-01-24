using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class RequestConfiguration : IEntityTypeConfiguration<Request>
{
    public void Configure(EntityTypeBuilder<Request> builder)
    {
        builder.HasOne(x => x.Client)
            .WithMany(x => x.Requests)
            .HasForeignKey(x => x.ClientId);

        builder.Property(x => x.Comment)
            .HasMaxLength(1024)
            .IsRequired(false);
    }
}
