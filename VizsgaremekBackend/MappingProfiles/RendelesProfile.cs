using AutoMapper;
using VizsgaremekBackend.Models;
using VizsgaremekBackend.Dtos;

namespace VizsgaremekBackend.MappingProfiles;

public class RendelesProfile : Profile
{
    public RendelesProfile()
    {
        CreateMap<RendelesReadDto, Rendeles>();
        CreateMap<Rendeles, RendelesReadDto>();
        
        CreateMap<RendelesWriteDto, Rendeles>();
        CreateMap<Rendeles, RendelesWriteDto>();
    }
}