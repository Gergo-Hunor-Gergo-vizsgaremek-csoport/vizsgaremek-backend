using AutoMapper;
using VizsgaremekBackend.Dtos;
using Type = VizsgaremekBackend.Models.Type;

namespace VizsgaremekBackend.MappingProfiles;

public class TypeProfile : Profile
{
    public TypeProfile()
    {
        CreateMap<Type, TypeReadDto>();
        CreateMap<TypeReadDto, Type>();

        CreateMap<TypeWriteDto, Type>();
        CreateMap<Type, TypeWriteDto>();
    }
    
}