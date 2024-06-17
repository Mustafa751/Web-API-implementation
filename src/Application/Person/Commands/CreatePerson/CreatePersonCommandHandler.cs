using MyPosTask.Application.Common.Interfaces;
using MyPosTask.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyPosTask.Application.Person.Commands.CreatePerson;

public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreatePersonCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        var inputAddresses = request.Addresses.Select(a => a.Address1).ToList();

        var existingAddresses = await _context.Addresses
            .AsNoTracking()
            .Where(a => inputAddresses.Contains(a.Address1))
            .ToListAsync(cancellationToken);

        var newAddresses = request.Addresses
            .Where(a => !existingAddresses.Select(ea => ea.Address1).Contains(a.Address1))
            .Select(a => new Address
            {
                Address1 = a.Address1
            }).ToList();

        _context.Addresses.AddRange(newAddresses);
        await _context.SaveChangesAsync(cancellationToken);

        var allAddresses = existingAddresses.Concat(newAddresses).ToList();

        var person = new MyPosTask.Domain.Entities.Person
        {
            Name = request.Name
        };

        await _context.People.AddAsync(person, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var personAddresses = request.Addresses.Select(a => new PersonAddress
        {
            PersonId = person.Id,
            AddressId = allAddresses.FirstOrDefault(aa => aa.Address1 == a.Address1)!.Id,
            Type = a.Type.ToString(),
            PhoneNumbers = a.PhoneNumbers.Select(p => new PhoneNumber
            {
                PhoneNumber1 = p.PhoneNumber1
            }).ToList()
        }).ToList();

        person.PersonAddresses = personAddresses;

        await _context.SaveChangesAsync(cancellationToken);

        return person.Id;
    }
}
