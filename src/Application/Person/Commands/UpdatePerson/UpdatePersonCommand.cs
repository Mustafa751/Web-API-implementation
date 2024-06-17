using Microsoft.Extensions.DependencyInjection.Person.Queries;
using MyPosTask.Application.Person.Models;

namespace MyPosTask.Application.Person.Commands.UpdatePerson;

public class UpdatePersonCommand : IRequest<PersonDto>
{
    public int Id { get; set; } // Keep the Id for the person
    public string Name { get; set; } = null!;
    public List<AddressDto> Addresses { get; set; } = new();
}
