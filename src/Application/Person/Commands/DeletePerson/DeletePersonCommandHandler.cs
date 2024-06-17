using Microsoft.Extensions.DependencyInjection.Person.Queries;
using MyPosTask.Application.Common.Exceptions;
using MyPosTask.Application.Common.Interfaces;
using MyPosTask.Domain.Enums;

namespace Microsoft.Extensions.DependencyInjection.Person.Commands.DeletePerson;

public class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand>
    {
        private readonly IApplicationDbContext _context;

        public DeletePersonCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(DeletePersonCommand request, CancellationToken cancellationToken)
        {
            var person = await _context.People
                .Include(p => p.Addresses)
                .ThenInclude(a => a.PhoneNumbers)
                .AsSplitQuery()
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (person == null)
            {
                throw new NotFoundException(nameof(Person), request.Id.ToString());
            }

            var originalRowVersion = _context.Entry(person).Property("RowVersion").OriginalValue as byte[];

            _context.People.Remove(person);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                var currentPerson = await _context.People
                    .AsNoTracking()
                    .Include(p => p.Addresses)
                    .ThenInclude(a => a.PhoneNumbers)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

                if (currentPerson == null)
                {
                    throw new NotFoundException(nameof(Person), request.Id.ToString());
                }

                var personDto = MapToPersonDto(currentPerson);

                throw new ConcurrencyException("The record you attempted to delete was modified by another user after you got the original value.", personDto);
            }
        }

        private PersonDto MapToPersonDto(MyPosTask.Domain.Entities.Person person)
        {
            return new PersonDto
            {
                Id = person.Id,
                Name = person.Name,
                Addresses = person.Addresses.Select(a => new PersonDto.AddressDto
                {
                    Id = a.Id,
                    Type = Enum.Parse<AddressType>(a.Type, true),
                    Address1 = a.Address1,
                    PhoneNumbers = a.PhoneNumbers.Select(p => new PersonDto.PhoneNumberDto
                    {
                        Id = p.Id,
                        PhoneNumber1 = p.PhoneNumber1
                    }).ToList()
                }).ToList(),
                RowVersion = _context.Entry(person).Property("RowVersion").CurrentValue as byte[]
            };
        }
    }
