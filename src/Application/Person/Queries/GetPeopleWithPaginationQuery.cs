using MyPosTask.Application.Common.Models;
using MyPosTask.Domain.Enums;

namespace Microsoft.Extensions.DependencyInjection.Person.Queries;

public class GetPeopleWithPaginationQuery : IRequest<PaginatedList<PersonDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Filter { get; set; }
}

public class PersonDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public List<AddressDto> Addresses { get; set; } = new();
    public byte[]? RowVersion { get; set; } = null!; // RowVersion for concurrency

    public class AddressDto
    {
        public int Id { get; set; }
        public AddressType Type { get; set; }
        public string Address1 { get; set; } = null!;
        public List<PhoneNumberDto> PhoneNumbers { get; set; } = new();
    }

    public class PhoneNumberDto
    {
        public int Id { get; set; }
        public string PhoneNumber1 { get; set; } = null!;
    }
}
