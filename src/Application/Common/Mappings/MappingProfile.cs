using AutoMapper;
using Microsoft.Extensions.DependencyInjection.Person.Queries;
using MyPosTask.Domain.Entities;
using MyPosTask.Domain.Enums;

namespace MyPosTask.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Domain.Entities.Person, PersonDto>()
            .ForMember(dest => dest.RowVersion, opt => opt.Ignore()) // Ignore RowVersion for specific mapping
            .ForMember(dest => dest.Addresses, opt => opt.MapFrom(src => src.PersonAddresses));

        CreateMap<PersonAddress, PersonDto.AddressDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AddressId))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<AddressType>(src.Type)))
            .ForMember(dest => dest.Address1, opt => opt.MapFrom(src => src.Address.Address1))
            .ForMember(dest => dest.PhoneNumbers, opt => opt.MapFrom(src => src.PhoneNumbers));

        CreateMap<PhoneNumber, PersonDto.PhoneNumberDto>();
    }
}
