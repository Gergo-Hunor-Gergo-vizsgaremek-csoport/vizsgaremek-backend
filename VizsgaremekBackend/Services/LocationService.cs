using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using Location = VizsgaremekBackend.Models.Location;

namespace VizsgaremekBackend.Services;

public class LocationService(VizsgaremekContext vizsgaremekContext, IMapper mapper)
{
    public async Task<List<LocationReadDto>> SearchAsync(string query, int limit, int offset)
    {
        if (limit < 0) throw new BadRequestException("Limit must be greater than or equal to 0.");
        if (offset < 0) throw new BadRequestException("Offset must be greater than or equal to 0.");

        query ??= string.Empty;

        return (await vizsgaremekContext.Locations
                .Where(x => x.Name.ToLower().Contains(query.ToLower()))
                .Skip(offset)
                .Take(limit)
                .ToArrayAsync())
            .Select(mapper.Map<LocationReadDto>)
            .ToList();
    }

    public async Task<LocationReadDto> GetAsync(Guid id)
    {
        Location result = await vizsgaremekContext.Locations.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Location), id);

        return mapper.Map<LocationReadDto>(result);
    }

    public async Task<LocationReadDto> PostAsync(LocationWriteDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Location newLocation = mapper.Map<Location>(dto);

        await vizsgaremekContext.Locations.AddAsync(newLocation);
        await vizsgaremekContext.SaveChangesAsync();

        return mapper.Map<LocationReadDto>(newLocation);
    }

    public async Task PutAsync(Guid id, LocationWriteDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Location oldLocation = await vizsgaremekContext.Locations.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Location), id);

        mapper.Map(dto, oldLocation);
        await vizsgaremekContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        Location location = await vizsgaremekContext.Locations.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Location), id);

        vizsgaremekContext.Locations.Remove(location);
        await vizsgaremekContext.SaveChangesAsync();
    }
}