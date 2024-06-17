using Microsoft.EntityFrameworkCore.ChangeTracking;
using MyPosTask.Domain.Entities;

namespace MyPosTask.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Person> People { get; }
    DbSet<Address> Addresses { get; }
    DbSet<PhoneNumber> PhoneNumbers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    
    EntityEntry Entry(object entity);
    
}
