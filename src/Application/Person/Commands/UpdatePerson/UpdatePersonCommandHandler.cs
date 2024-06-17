using Microsoft.Extensions.DependencyInjection.Person.Queries;
using MyPosTask.Application.Common.Exceptions;
using MyPosTask.Application.Common.Interfaces;
using MyPosTask.Application.Person.Models;
using MyPosTask.Domain.Entities;
using MyPosTask.Domain.Enums;

namespace MyPosTask.Application.Person.Commands.UpdatePerson;

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
            .Include(p => p.PersonAddresses)
                .ThenInclude(pa => pa.Address)
            .Include(p => p.PersonAddresses)
                .ThenInclude(pa => pa.PhoneNumbers)
            .AsTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (person == null)
        {
            throw new NotFoundException(nameof(Microsoft.Extensions.DependencyInjection.Person), request.Id.ToString());
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
                .Include(p => p.PersonAddresses)
                    .ThenInclude(pa => pa.Address)
                .Include(p => p.PersonAddresses)
                    .ThenInclude(pa => pa.PhoneNumbers)
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (currentPerson == null)
            {
                throw new NotFoundException(nameof(Microsoft.Extensions.DependencyInjection.Person), request.Id.ToString());
            }

            var personDto = MapToPersonDto(currentPerson);

            // Corrected exception call with only required arguments
            throw new ConcurrencyException<PersonDto>("The record you attempted to edit was modified by another user after you got the original value.", personDto);
        }

        var updatedPersonDto = MapToPersonDto(person);
        return updatedPersonDto;
    }

    private void SyncAddresses(MyPosTask.Domain.Entities.Person person, List<AddressDto> requestAddresses)
    {
        var requestAddressIds = requestAddresses.Select(a => a.Address1).ToHashSet();

        // Remove PersonAddresses not in request
        var personAddressesToRemove = person.PersonAddresses.Where(pa => !requestAddressIds.Contains(pa.Address.Address1)).ToList();
        _context.PersonAddresses.RemoveRange(personAddressesToRemove);

        foreach (var addressDto in requestAddresses)
        {
            var personAddress = person.PersonAddresses.FirstOrDefault(pa => pa.Address.Address1 == addressDto.Address1);
            if (personAddress != null)
            {
                personAddress.Type = addressDto.Type.ToString();

                var existingPhoneNumbers = personAddress.PhoneNumbers.ToList();
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
                        personAddress.PhoneNumbers.Add(new PhoneNumber
                        {
                            PhoneNumber1 = phoneNumberDto.PhoneNumber1,
                            PersonAddressId = personAddress.Id // Ensure the foreign key is set correctly
                        });
                    }
                }
            }
            else
            {
                var address = _context.Addresses.FirstOrDefault(a => a.Address1 == addressDto.Address1) ?? new Address
                {
                    Address1 = addressDto.Address1
                };

                person.PersonAddresses.Add(new PersonAddress
                {
                    Type = addressDto.Type.ToString(),
                    Address = address,
                    PhoneNumbers = addressDto.PhoneNumbers.Select(p => new PhoneNumber
                    {
                        PhoneNumber1 = p.PhoneNumber1
                    }).ToList()
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
            Addresses = person.PersonAddresses.Select(pa => new PersonDto.AddressDto
            {
                Id = pa.Id,
                Type = Enum.Parse<AddressType>(pa.Type, true),
                Address1 = pa.Address.Address1,
                PhoneNumbers = pa.PhoneNumbers.Select(p => new PersonDto.PhoneNumberDto
                {
                    Id = p.Id,
                    PhoneNumber1 = p.PhoneNumber1
                }).ToList()
            }).ToList(),
            RowVersion = _context.Entry(person).Property("RowVersion").CurrentValue as byte[]
        };
    }
}
