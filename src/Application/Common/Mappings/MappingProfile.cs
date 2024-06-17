using Microsoft.Extensions.DependencyInjection.Person.Queries;
using MyPosTask.Domain.Entities;

namespace MyPosTask.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Person, PersonDto>()
            .ForMember(dest => dest.RowVersion, opt => opt.Ignore()); // Ignore RowVersion for specific mapping

        CreateMap<Address, PersonDto.AddressDto>();
        CreateMap<PhoneNumber, PersonDto.PhoneNumberDto>();
    }
}
