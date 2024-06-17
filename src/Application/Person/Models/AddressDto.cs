using MyPosTask.Domain.Enums;

namespace MyPosTask.Application.Person.Models;

public class AddressDto
{
    public AddressType Type { get; set; }
    public string Address1 { get; set; } = null!;
    public List<PhoneNumberDto> PhoneNumbers { get; set; } = new();
}
