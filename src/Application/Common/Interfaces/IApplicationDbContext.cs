using Microsoft.EntityFrameworkCore.ChangeTracking;
using MyPosTask.Domain.Entities;

namespace MyPosTask.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Domain.Entities.Person> People { get; }
    DbSet<Address> Addresses { get; }
    DbSet<PhoneNumber> PhoneNumbers { get; }
    
    DbSet<PersonAddress> PersonAddresses { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    
    EntityEntry Entry(object entity);
    
}
