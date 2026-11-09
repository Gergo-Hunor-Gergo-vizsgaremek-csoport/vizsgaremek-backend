using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using Location = VizsgaremekBackend.Models.Location;

namespace VizsgaremekBackend.Services;

public class LocationService(VizsgaremekContext vizsgaremekContext, IMapper mapper)
{
    /// <summary>
    /// Searches for locations by name using pagination.
    /// </summary>
    /// <param name="query">The search text matched against location names (case-insensitive).</param>
    /// <param name="limit">The maximum number of results to return.</param>
    /// <param name="offset">The number of results to skip.</param>
    /// <returns>A list of <see cref="LocationReadDto"/> items matching the search criteria.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="query"/> is <c>null</c>.</exception>
    /// <exception cref="BadRequestException">Thrown when <paramref name="limit"/> or <paramref name="offset"/> is negative.</exception>
    public async Task<List<LocationReadDto>> SearchAsync(string query, int limit, int offset)
    {
        ArgumentNullException.ThrowIfNull(query);
        if (limit < 0) throw new BadRequestException("Limit must be greater than or equal to 0.");
        if (offset < 0) throw new BadRequestException("Offset must be greater than or equal to 0.");

        return (await vizsgaremekContext.Locations
                .Where(x => x.Name.ToLower().Contains(query.ToLower()))
                .Skip(offset)
                .Take(limit)
                .ToArrayAsync())
            .Select(mapper.Map<LocationReadDto>)
            .ToList();
    }

    /// <summary>
    /// Retrieves a location by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the location to retrieve.</param>
    /// <returns>The mapped <see cref="LocationReadDto"/> for the requested location.</returns>
    /// <exception cref="NotFoundException">Thrown when no location exists with the specified <paramref name="id"/>.</exception>
    public async Task<LocationReadDto> GetAsync(Guid id)
    {
        Location result = await vizsgaremekContext.Locations.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Location), id);

        return mapper.Map<LocationReadDto>(result);
    }

    /// <summary>
    /// Creates a new location.
    /// </summary>
    /// <param name="dto">The data used to create the location.</param>
    /// <returns>The mapped <see cref="LocationReadDto"/> representing the created location.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is <c>null</c>.</exception>
    public async Task<LocationReadDto> PostAsync(LocationWriteDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Location newLocation = mapper.Map<Location>(dto);

        await vizsgaremekContext.Locations.AddAsync(newLocation);
        await vizsgaremekContext.SaveChangesAsync();

        return mapper.Map<LocationReadDto>(newLocation);
    }

    /// <summary>
    /// Updates an existing location.
    /// </summary>
    /// <param name="id">The unique identifier of the location to update.</param>
    /// <param name="dto">The updated location data.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is <c>null</c>.</exception>
    /// <exception cref="NotFoundException">Thrown when no location exists with the specified <paramref name="id"/>.</exception>
    public async Task PutAsync(Guid id, LocationWriteDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Location oldLocation = await vizsgaremekContext.Locations.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Location), id);

        mapper.Map(dto, oldLocation);
        await vizsgaremekContext.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a location by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the location to delete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    /// <exception cref="NotFoundException">Thrown when no location exists with the specified <paramref name="id"/>.</exception>
    public async Task DeleteAsync(Guid id)
    {
        Location location = await vizsgaremekContext.Locations.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Location), id);

        vizsgaremekContext.Locations.Remove(location);
        await vizsgaremekContext.SaveChangesAsync();
    }
}