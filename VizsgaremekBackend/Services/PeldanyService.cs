using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.Services;

public class PeldanyService(VizsgaremekContext vizsgaremekContext, IMapper mapper)
{
    /// <summary>
    /// Retrieves an item instance (példány) by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the instance to retrieve.</param>
    /// <returns>The mapped <see cref="PeldanyReadDto"/> for the requested instance.</returns>
    /// <exception cref="NotFoundException">Thrown when no instance exists with the specified <paramref name="id"/>.</exception>
    public async Task<PeldanyReadDto> GetAsync(Guid id)
    {
        Peldany result = await vizsgaremekContext.Peldanys.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Peldany), id);

        return mapper.Map<PeldanyReadDto>(result);
    }

    /// <summary>
    /// Creates a new item instance (példány).
    /// </summary>
    /// <param name="dto">The data used to create the instance.</param>
    /// <returns>The mapped <see cref="PeldanyReadDto"/> representing the created instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is <c>null</c>.</exception>
    public async Task<PeldanyReadDto> PostAsync(PeldanyWriteDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Peldany newPeldany = mapper.Map<Peldany>(dto);

        await vizsgaremekContext.Peldanys.AddAsync(newPeldany);
        await vizsgaremekContext.SaveChangesAsync();

        return mapper.Map<PeldanyReadDto>(newPeldany);
    }

    /// <summary>
    /// Updates an existing item instance (példány).
    /// </summary>
    /// <param name="id">The unique identifier of the instance to update.</param>
    /// <param name="dto">The updated instance data.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is <c>null</c>.</exception>
    /// <exception cref="NotFoundException">Thrown when no instance exists with the specified <paramref name="id"/>.</exception>
    public async Task PutAsync(Guid id, PeldanyWriteDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Peldany oldPeldany = await vizsgaremekContext.Peldanys.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Peldany), id);

        mapper.Map(dto, oldPeldany);
        await vizsgaremekContext.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes an item instance (példány) by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the instance to delete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    /// <exception cref="NotFoundException">Thrown when no instance exists with the specified <paramref name="id"/>.</exception>
    public async Task DeleteAsync(Guid id)
    {
        Peldany peldany = await vizsgaremekContext.Peldanys.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Peldany), id);

        vizsgaremekContext.Peldanys.Remove(peldany);
        await vizsgaremekContext.SaveChangesAsync();
    }
}