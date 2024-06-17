using Microsoft.EntityFrameworkCore;
using MyPosTask.Application.Common.Interfaces;
using MyPosTask.Domain.Entities;

namespace MyPosTask.Infrastructure.Persistence
{
    public partial class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Address> Addresses { get; set; } = null!;
        public virtual DbSet<Person> People { get; set; } = null!;
        public virtual DbSet<PhoneNumber> PhoneNumbers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("addresses_pkey");

                entity.ToTable("addresses");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Address1).HasColumnName("address");
                entity.Property(e => e.PeopleId).HasColumnName("people_id");
                entity.Property(e => e.Type)
                      .HasColumnName("type")
                      .IsRequired();

                entity.HasOne(d => d.People)
                      .WithMany(p => p.Addresses)
                      .HasForeignKey(d => d.PeopleId)
                      .OnDelete(DeleteBehavior.Cascade) // Configure cascading delete
                      .HasConstraintName("addresses_people_id_fkey");
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("people_pkey");

                entity.ToTable("people");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property<byte[]>("RowVersion")
                    .IsRowVersion()
                    .HasColumnName("row_version");
            });

            modelBuilder.Entity<PhoneNumber>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("phone_numbers_pkey");

                entity.ToTable("phone_numbers");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.AddressId).HasColumnName("address_id");
                entity.Property(e => e.PhoneNumber1).HasColumnName("phone_number");

                entity.HasOne(d => d.Address)
                      .WithMany(p => p.PhoneNumbers)
                      .HasForeignKey(d => d.AddressId)
                      .OnDelete(DeleteBehavior.Cascade) // Configure cascading delete
                      .HasConstraintName("phone_numbers_address_id_fkey");
            });

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
