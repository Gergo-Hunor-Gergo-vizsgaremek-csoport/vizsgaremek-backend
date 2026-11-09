using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using Type = VizsgaremekBackend.Models.Type;

namespace VizsgaremekBackend.Services;

public class TypeService(VizsgaremekContext vizsgaremekContext, IMapper mapper)
{
    /// <summary>
    /// Searches for types by name using pagination and returns a paged result.
    /// </summary>
    /// <param name="query">The search text matched against type names (case-insensitive).</param>
    /// <param name="limit">The maximum number of items to return in the page.</param>
    /// <param name="offset">The number of items to skip.</param>
    /// <returns>A <see cref="PagedDto{T}"/> containing the matching <see cref="TypeReadDto"/> items and the total page count.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="query"/> is <c>null</c>.</exception>
    /// <exception cref="BadRequestException">Thrown when <paramref name="limit"/> or <paramref name="offset"/> is negative.</exception>
    public async Task<PagedDto<TypeReadDto>> SearchAsync(string query, int limit, int offset)
    {
        ArgumentNullException.ThrowIfNull(query);
        if (limit < 0) throw new BadRequestException("Limit must be greater than or equal to 0.");
        if (offset < 0) throw new BadRequestException("Offset must be greater than or equal to 0.");

        int queryCount = await vizsgaremekContext.Types
            .Where(x => x.Name.ToLower().Contains(query.ToLower()))
            .CountAsync();

        int pageCount = Convert.ToInt32(Math.Ceiling(queryCount/(double)limit));

        TypeReadDto[] items = (await vizsgaremekContext.Types
                .Where(x => x.Name.ToLower().Contains(query.ToLower()))
                .Skip(offset)
                .Take(limit)
                .ToArrayAsync())
            .Select(mapper.Map<TypeReadDto>)
            .ToArray();

        return new PagedDto<TypeReadDto>(items,pageCount);
    }

    /// <summary>
    /// Retrieves a type by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the type to retrieve.</param>
    /// <returns>The mapped <see cref="TypeReadDto"/> for the requested type.</returns>
    /// <exception cref="NotFoundException">Thrown when no type exists with the specified <paramref name="id"/>.</exception>
    public async Task<TypeReadDto> GetAsync(Guid id)
    {
        Type result = await vizsgaremekContext.Types.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Type), id);

        return mapper.Map<TypeReadDto>(result);
    }

    /// <summary>
    /// Creates a new type.
    /// </summary>
    /// <param name="dto">The data used to create the type.</param>
    /// <returns>The mapped <see cref="TypeReadDto"/> representing the created type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is <c>null</c>.</exception>
    public async Task<TypeReadDto> PostAsync(TypeWriteDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Type newType = mapper.Map<Type>(dto);

        await vizsgaremekContext.Types.AddAsync(newType);
        await vizsgaremekContext.SaveChangesAsync();

        return mapper.Map<TypeReadDto>(newType);
    }

    /// <summary>
    /// Updates an existing type.
    /// </summary>
    /// <param name="id">The unique identifier of the type to update.</param>
    /// <param name="dto">The updated type data.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is <c>null</c>.</exception>
    /// <exception cref="NotFoundException">Thrown when no type exists with the specified <paramref name="id"/>.</exception>
    public async Task PutAsync(Guid id, TypeWriteDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Type oldType = await vizsgaremekContext.Types.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Type), id);

        mapper.Map(dto, oldType);
        await vizsgaremekContext.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a type by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the type to delete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    /// <exception cref="NotFoundException">Thrown when no type exists with the specified <paramref name="id"/>.</exception>
    public async Task DeleteAsync(Guid id)
    {
        Type type = await vizsgaremekContext.Types.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Type), id);

        vizsgaremekContext.Types.Remove(type);
        await vizsgaremekContext.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves available item instances (példányok) for a type using pagination.
    /// </summary>
    /// <param name="id">The unique identifier of the type whose instances are requested.</param>
    /// <param name="limit">The maximum number of instances to return.</param>
    /// <param name="offset">The number of instances to skip.</param>
    /// <returns>An array of <see cref="PeldanyReadDto"/> items that belong to the type and have no active loan.</returns>
    /// <exception cref="BadRequestException">Thrown when <paramref name="limit"/> or <paramref name="offset"/> is negative.</exception>
    /// <exception cref="NotFoundException">Thrown when no type exists with the specified <paramref name="id"/>.</exception>
    public async Task<PeldanyReadDto[]> GetPeldanys(Guid id, int limit, int offset)
    {
        if (limit < 0) throw new BadRequestException("Limit must be greater than or equal to 0.");
        if (offset < 0) throw new BadRequestException("Offset must be greater than or equal to 0.");

        Type? type = await vizsgaremekContext.Types
            .Include(x => x.Peldanys)
            .SingleOrDefaultAsync(x => x.Id == id);

        if (type is null) throw new NotFoundException(nameof(Type), id);

        return type.Peldanys
            .Where(x => x.ActiveKolcsonzesId == null)
            .Skip(offset)
            .Take(limit)
            .Select(x => mapper.Map<PeldanyReadDto>(x))
            .ToArray();
    }
}