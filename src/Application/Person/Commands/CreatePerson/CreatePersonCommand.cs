using MyPosTask.Domain.Enums;

namespace Microsoft.Extensions.DependencyInjection.People.Commands;

public class CreatePersonCommand : IRequest<int>
{
    public string Name { get; set; } = null!;
    public List<AddressDto> Addresses { get; set; } = new();

    public class AddressDto
    {
        public AddressType Type { get; set; }
        public string Address1 { get; set; } = null!;
        public List<PhoneNumberDto> PhoneNumbers { get; set; } = new();
    }

    public class PhoneNumberDto
    {
        public string PhoneNumber1 { get; set; } = null!;
    }
}

