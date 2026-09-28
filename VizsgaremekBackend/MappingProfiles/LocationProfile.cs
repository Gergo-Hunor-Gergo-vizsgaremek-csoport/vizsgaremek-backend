using AutoMapper;
using VizsgaremekBackend.Dtos;
using Location = VizsgaremekBackend.Models.Location;

namespace VizsgaremekBackend.MappingProfiles;

public class LocationProfile: Profile
{
    public LocationProfile()
    {
        CreateMap<Location, LocationReadDto>();
        CreateMap<LocationReadDto, Location>();

        CreateMap<LocationWriteDto, Location>();
        CreateMap<Location, LocationWriteDto>();
    }
}