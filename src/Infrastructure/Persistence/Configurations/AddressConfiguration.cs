using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyPosTask.Domain.Entities;

namespace MyPosTask.Infrastructure.Persistence.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(e => e.Id).HasName("addresses_pkey");

            builder.ToTable("addresses");

            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.Address1).HasColumnName("address");

            builder.HasMany(e => e.PersonAddresses)
                .WithOne(pa => pa.Address)
                .HasForeignKey(pa => pa.AddressId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
