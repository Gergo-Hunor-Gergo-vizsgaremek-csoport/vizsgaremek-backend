using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using Kolcsonzes = VizsgaremekBackend.Models.Kolcsonzes;

namespace VizsgaremekBackend.Services;

public class KolcsonzesService(VizsgaremekContext vizsgaremekContext, IMapper mapper)
{
    /// <summary>
    /// Retrieves a loan (kölcsönzés) by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the loan to retrieve.</param>
    /// <returns>The mapped <see cref="KolcsonzesReadDto"/> for the requested loan.</returns>
    /// <exception cref="NotFoundException">Thrown when no loan exists with the specified <paramref name="id"/>.</exception>
    public async Task<KolcsonzesReadDto> GetAsync(Guid id)
    {
        Kolcsonzes result = await vizsgaremekContext.Kolcsonzess.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Kolcsonzes), id);

        return mapper.Map<KolcsonzesReadDto>(result);
    }

    /// <summary>
    /// Creates a new loan (kölcsönzés).
    /// </summary>
    /// <param name="dto">The data used to create the loan.</param>
    /// <returns>The mapped <see cref="KolcsonzesReadDto"/> representing the created loan.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is <c>null</c>.</exception>
    public async Task<KolcsonzesReadDto> PostAsync(KolcsonzesWriteDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Kolcsonzes newKolcsonzes = mapper.Map<Kolcsonzes>(dto);

        await vizsgaremekContext.Kolcsonzess.AddAsync(newKolcsonzes);
        await vizsgaremekContext.SaveChangesAsync();

        return mapper.Map<KolcsonzesReadDto>(newKolcsonzes);
    }

    /// <summary>
    /// Updates an existing loan (kölcsönzés).
    /// </summary>
    /// <param name="id">The unique identifier of the loan to update.</param>
    /// <param name="dto">The updated loan data.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is <c>null</c>.</exception>
    /// <exception cref="NotFoundException">Thrown when no loan exists with the specified <paramref name="id"/>.</exception>
    public async Task PutAsync(Guid id, KolcsonzesWriteDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Kolcsonzes oldKolcsonzes = await vizsgaremekContext.Kolcsonzess.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Kolcsonzes), id);

        mapper.Map(dto, oldKolcsonzes);
        await vizsgaremekContext.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a loan (kölcsönzés) by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the loan to delete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    /// <exception cref="NotFoundException">Thrown when no loan exists with the specified <paramref name="id"/>.</exception>
    public async Task DeleteAsync(Guid id)
    {
        Kolcsonzes kolcsonzes = await vizsgaremekContext.Kolcsonzess.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Kolcsonzes), id);

        vizsgaremekContext.Kolcsonzess.Remove(kolcsonzes);
        await vizsgaremekContext.SaveChangesAsync();
    }
}