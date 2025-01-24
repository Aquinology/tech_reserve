using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(x => x.ApplicationUserId);

        builder.Property(x => x.Email)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.FirstName)
            .HasMaxLength(128)
            .IsRequired(false);

        builder.Property(x => x.LastName)
            .HasMaxLength(128)
            .IsRequired(false);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(128)
            .IsRequired(false);

        builder.Property(x => x.OtherInfo)
            .HasMaxLength(256)
            .IsRequired(false);
    }
}
