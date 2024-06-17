using Microsoft.Extensions.DependencyInjection.Person.Queries;
using MyPosTask.Application.Common.Models;

namespace MyPosTask.Application.Common.Mappings;

public class PersonResponseMapper
{
    public static object MapToResponse(PaginatedList<PersonDto> result)
    {
        return new
        {
            page_size = result.PageSize,
            page_number = result.PageNumber,
            total_count = result.TotalCount,
            total_pages = result.TotalPages,
            people = result.Items.Select(p => new
            {
                id = p.Id,
                name = p.Name,
                addresses = p.Addresses.Select(a => new
                {
                    type = a.Type,
                    address = a.Address1,
                    phone_numbers = a.PhoneNumbers.Select(ph => ph.PhoneNumber1)
                })
            })
        };
    }
}
