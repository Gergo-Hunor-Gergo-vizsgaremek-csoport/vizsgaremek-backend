using AutoMapper;
using VizsgaremekBackend.Models;
using VizsgaremekBackend.Dtos;

namespace VizsgaremekBackend.MappingProfiles;

public class PeldanyProfile : Profile
{
    public PeldanyProfile()
    {
        CreateMap<Peldany, PeldanyReadDto>();
        CreateMap<PeldanyReadDto, Peldany>();

        CreateMap<PeldanyWriteDto, Peldany>();
        CreateMap<Peldany, PeldanyWriteDto>();
    }
}