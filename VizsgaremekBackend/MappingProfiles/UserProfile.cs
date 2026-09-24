using AutoMapper;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.MappingProfiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserReadDto>();
        CreateMap<UserReadDto, User>();

        CreateMap<User, UserWriteDto>();
        CreateMap<UserWriteDto, User>();
    }
    
}