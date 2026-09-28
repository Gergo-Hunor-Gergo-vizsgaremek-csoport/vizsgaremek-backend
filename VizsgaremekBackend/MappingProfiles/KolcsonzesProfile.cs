using AutoMapper;
using VizsgaremekBackend.Dtos;
using Kolcsonzes = VizsgaremekBackend.Models.Kolcsonzes;

namespace VizsgaremekBackend.MappingProfiles;

public class KolcsonzesProfile : Profile
{
    public KolcsonzesProfile()
    {
        CreateMap<Kolcsonzes, KolcsonzesReadDto>();
        CreateMap<KolcsonzesReadDto, Kolcsonzes>();

        CreateMap<KolcsonzesWriteDto, Kolcsonzes>();
        CreateMap<Kolcsonzes, KolcsonzesWriteDto>();
    }
}