using MyPosTask.Application.Common.Interfaces;
using MyPosTask.Domain.Entities;

namespace Microsoft.Extensions.DependencyInjection.People.Commands;

public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreatePersonCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        var person = new MyPosTask.Domain.Entities.Person
        {
            Name = request.Name,
            Addresses = request.Addresses.Select(a => new Address
            {
                Type = a.Type.ToString(), // Convert enum to string
                Address1 = a.Address1,
                PhoneNumbers = a.PhoneNumbers.Select(p => new PhoneNumber
                {
                    PhoneNumber1 = p.PhoneNumber1
                }).ToList()
            }).ToList()
        };

        await _context.People.AddAsync(person, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return person.Id;
    }
}
