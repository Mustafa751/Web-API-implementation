using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyPosTask.Domain.Entities;

namespace MyPosTask.Infrastructure.Persistence.Configurations
{
    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.HasKey(e => e.Id).HasName("people_pkey");

            builder.ToTable("people");

            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.Name).HasColumnName("name");
            builder.Property<byte[]>("RowVersion")
                .IsRowVersion()
                .HasColumnName("row_version");

            builder.HasMany(e => e.PersonAddresses)
                .WithOne(pa => pa.Person)
                .HasForeignKey(pa => pa.PersonId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
