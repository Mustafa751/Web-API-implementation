using MyPosTask.Application.Person.Models;
using MyPosTask.Domain.Enums;

namespace MyPosTask.Application.Person.Commands.CreatePerson;

public class CreatePersonCommand : IRequest<int>
{
    public string Name { get; set; } = null!;
    public List<AddressDto> Addresses { get; set; } = new();
    
}

