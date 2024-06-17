using MyPosTask.Application.Common.Interfaces;
using MyPosTask.Domain.Entities;
using Microsoft.Extensions.DependencyInjection.Person.Queries;
using MyPosTask.Application.Common.Exceptions;
using MyPosTask.Domain.Enums;

namespace Microsoft.Extensions.DependencyInjection.People.Commands.UpdatePerson;

public class UpdatePersonCommandHandler : IRequestHandler<UpdatePersonCommand, PersonDto>
{
    private readonly IApplicationDbContext _context;

    public UpdatePersonCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PersonDto> Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
    {
        var person = await _context.People
            .Include(p => p.Addresses)
            .ThenInclude(a => a.PhoneNumbers)
            .AsTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (person == null)
        {
            throw new NotFoundException(nameof(Person), request.Id.ToString());
        }

        // Update the person's name
        person.Name = request.Name;

        // Sync Addresses and PhoneNumbers
        SyncAddresses(person, request.Addresses);

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
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (currentPerson == null)
            {
                throw new NotFoundException(nameof(Person), request.Id.ToString());
            }

            var personDto = MapToPersonDto(currentPerson);

            // Corrected exception call with only required arguments
            throw new ConcurrencyException("The record you attempted to edit was modified by another user after you got the original value.", personDto);
        }

        var updatedPersonDto = MapToPersonDto(person);
        return updatedPersonDto;
    }

    private void SyncAddresses(MyPosTask.Domain.Entities.Person person, List<UpdatePersonCommand.AddressDto> requestAddresses)
    {
        var requestAddressIds = requestAddresses.Select(a => a.Address1).ToHashSet();

        // Remove addresses not in request
        var addressesToRemove = person.Addresses.Where(a => !requestAddressIds.Contains(a.Address1)).ToList();
        _context.Addresses.RemoveRange(addressesToRemove);

        foreach (var addressDto in requestAddresses)
        {
            var address = person.Addresses.FirstOrDefault(a => a.Address1 == addressDto.Address1);
            if (address != null)
            {
                address.Type = addressDto.Type.ToString();
                address.Address1 = addressDto.Address1;

                var existingPhoneNumbers = address.PhoneNumbers.ToList();
                var requestPhoneNumberIds = addressDto.PhoneNumbers.Select(p => p.PhoneNumber1).ToHashSet();

                // Remove phone numbers not in request
                var phoneNumbersToRemove = existingPhoneNumbers.Where(p => !requestPhoneNumberIds.Contains(p.PhoneNumber1)).ToList();
                _context.PhoneNumbers.RemoveRange(phoneNumbersToRemove);

                foreach (var phoneNumberDto in addressDto.PhoneNumbers)
                {
                    var phoneNumber = existingPhoneNumbers.FirstOrDefault(p => p.PhoneNumber1 == phoneNumberDto.PhoneNumber1);
                    if (phoneNumber != null)
                    {
                        phoneNumber.PhoneNumber1 = phoneNumberDto.PhoneNumber1;
                    }
                    else
                    {
                        address.PhoneNumbers.Add(new PhoneNumber
                        {
                            PhoneNumber1 = phoneNumberDto.PhoneNumber1
                        });
                    }
                }
            }
            else
            {
                person.Addresses.Add(new Address
                {
                    Type = addressDto.Type.ToString(),
                    Address1 = addressDto.Address1,
                    PhoneNumbers = addressDto.PhoneNumbers.Select(p => new PhoneNumber
                    {
                        PhoneNumber1 = p.PhoneNumber1                    }).ToList()
                });
            }
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
                    
