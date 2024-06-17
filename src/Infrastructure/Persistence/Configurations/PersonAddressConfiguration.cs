using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyPosTask.Domain.Entities;

namespace MyPosTask.Infrastructure.Persistence.Configurations
{
    public class PersonAddressConfiguration : IEntityTypeConfiguration<PersonAddress>
    {
        public void Configure(EntityTypeBuilder<PersonAddress> builder)
        {
            builder.HasKey(e => e.Id).HasName("person_addresses_pkey");

            builder.ToTable("person_addresses");

            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.PersonId).HasColumnName("person_id");
            builder.Property(e => e.AddressId).HasColumnName("address_id");
            builder.Property(e => e.Type)
                .HasColumnName("type")
                .IsRequired();

            builder.HasOne(e => e.Person)
                .WithMany(p => p.PersonAddresses)
                .HasForeignKey(e => e.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Address)
                .WithMany(a => a.PersonAddresses)
                .HasForeignKey(e => e.AddressId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.PhoneNumbers)
                .WithOne(p => p.PersonAddress)
                .HasForeignKey(p => p.PersonAddressId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
