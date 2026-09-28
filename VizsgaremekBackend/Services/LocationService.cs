using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using Location = VizsgaremekBackend.Models.Location;

namespace VizsgaremekBackend.Services;

public class LocationService(VizsgaremekContext vizsgaremekContext, IMapper mapper)
{
    public async Task<List<LocationReadDto>> SearchAsync(string query, int limit, int offset)
    {
        return ( await vizsgaremekContext.Locations
                .Where(x => x.Name.Contains(query))
                .Skip(offset)
                .Take(limit)
                .ToArrayAsync())
            .Select(mapper.Map<LocationReadDto>)
            .ToList();
    }
    
    public async Task<LocationReadDto> GetAsync(Guid id)
    {
        Location result = await vizsgaremekContext.Locations.SingleAsync(x => x.Id == id);
        
        return mapper.Map<LocationReadDto>(result);
    }
    
    public async Task PostAsync(LocationWriteDto dto)
    {
        Location newLocation = mapper.Map<Location>(dto);
        
        await vizsgaremekContext.Locations.AddAsync(newLocation);
        
        await vizsgaremekContext.SaveChangesAsync();
    }
    
    public async Task PutAsync(Guid id, LocationWriteDto dto)
    {
        Location oldLocation = await vizsgaremekContext.Locations.SingleAsync(x => x.Id == id);
        
        mapper.Map(dto, oldLocation);
        
        await vizsgaremekContext.SaveChangesAsync();
        
        
        
    }
    
    public async Task DeleteAsync(Guid id)
    {
        Location location = await vizsgaremekContext.Locations.SingleAsync(x => x.Id == id);
        
        vizsgaremekContext.Locations.Remove(location);
        
        await vizsgaremekContext.SaveChangesAsync();
    }
}