using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyPosTask.Domain.Entities;

namespace MyPosTask.Infrastructure.Persistence.Configurations
{
    public class PhoneNumberConfiguration : IEntityTypeConfiguration<PhoneNumber>
    {
        public void Configure(EntityTypeBuilder<PhoneNumber> builder)
        {
            builder.HasKey(e => e.Id).HasName("phone_numbers_pkey");

            builder.ToTable("phone_numbers");

            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.PersonAddressId).HasColumnName("person_address_id");
            builder.Property(e => e.PhoneNumber1).HasColumnName("phone_number");

            builder.HasOne(d => d.PersonAddress)
                .WithMany(p => p.PhoneNumbers)
                .HasForeignKey(d => d.PersonAddressId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("phone_numbers_person_address_id_fkey");
        }
    }
}
