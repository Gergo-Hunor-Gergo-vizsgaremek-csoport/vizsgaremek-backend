using AutoMapper;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.MappingProfiles;

public class LogProfile :Profile
{
    public LogProfile()
    {
        CreateMap<LogReadDto, Log>();
        CreateMap<Log, LogReadDto>();
        
        CreateMap<LogWriteDto, Log>();
        CreateMap<Log, LogWriteDto>();
    }
}