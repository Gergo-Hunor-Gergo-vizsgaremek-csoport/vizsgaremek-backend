using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.Services;

public class RendelesService(VizsgaremekContext vizsgaremekContext, IMapper mapper)
{
    /// <summary>
    /// Retrieves an order (rendelés) by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the order to retrieve.</param>
    /// <returns>The mapped <see cref="RendelesReadDto"/> for the requested order.</returns>
    /// <exception cref="NotFoundException">Thrown when no order exists with the specified <paramref name="id"/>.</exception>
    public async Task<RendelesReadDto> GetAsync(Guid id)
    {
        Rendeles result = await vizsgaremekContext.Rendeleses.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Rendeles), id);

        return mapper.Map<RendelesReadDto>(result);
    }

    /// <summary>
    /// Creates a new order (rendelés).
    /// </summary>
    /// <param name="dto">The data used to create the order.</param>
    /// <returns>The mapped <see cref="RendelesReadDto"/> representing the created order.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is <c>null</c>.</exception>
    public async Task<RendelesReadDto> PostAsync(RendelesWriteDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Rendeles newRendeles = mapper.Map<Rendeles>(dto);
        await vizsgaremekContext.Rendeleses.AddAsync(newRendeles);
        await vizsgaremekContext.SaveChangesAsync();
        return mapper.Map<RendelesReadDto>(newRendeles);
    }

    /// <summary>
    /// Updates an existing order (rendelés).
    /// </summary>
    /// <param name="id">The unique identifier of the order to update.</param>
    /// <param name="dto">The updated order data.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is <c>null</c>.</exception>
    /// <exception cref="NotFoundException">Thrown when no order exists with the specified <paramref name="id"/>.</exception>
    public async Task PutAsync(Guid id, RendelesWriteDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Rendeles oldRendeles = await vizsgaremekContext.Rendeleses.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Rendeles), id);

        mapper.Map(dto, oldRendeles);
        await vizsgaremekContext.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes an order (rendelés) by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the order to delete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    /// <exception cref="NotFoundException">Thrown when no order exists with the specified <paramref name="id"/>.</exception>
    public async Task DeleteAsync(Guid id)
    {
        Rendeles rendeles = await vizsgaremekContext.Rendeleses.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Rendeles), id);

        vizsgaremekContext.Rendeleses.Remove(rendeles);
        await vizsgaremekContext.SaveChangesAsync();
    }

    /// <summary>
    /// Assigns item instances (példányok) to an order by creating loans (kölcsönzések) from the provided assignment data.
    /// </summary>
    /// <param name="rendelesId">The unique identifier of the target order.</param>
    /// <param name="kolcsonzesWriteDtos">The loan data for each assignment to create. Each entry is mapped to a loan and linked to its referenced instance.</param>
    /// <returns>An array of <see cref="KolcsonzesReadDto"/> items representing the created loans, in the same order as the input.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="kolcsonzesWriteDtos"/> is <c>null</c>.</exception>
    /// <exception cref="BadRequestException">Thrown when <paramref name="kolcsonzesWriteDtos"/> is empty or contains a <c>null</c> element.</exception>
    /// <exception cref="NotFoundException">Thrown when the order or a referenced instance (példány) does not exist.</exception>
    /// <exception cref="ConflictException">Thrown when a referenced instance already has an active loan.</exception>
    /// <remarks>
    /// For each assignment, the method verifies the order exists, resolves the referenced instance, ensures the instance has no active loan,
    /// sets the instance's active loan reference, and adds the new loan. All changes are persisted in a single <c>SaveChangesAsync</c> call.
    /// </remarks>
    public async Task<KolcsonzesReadDto[]> AssignPeldanysAsync(Guid rendelesId, KolcsonzesWriteDto[] kolcsonzesWriteDtos)
    {
        ArgumentNullException.ThrowIfNull(kolcsonzesWriteDtos);
        if (kolcsonzesWriteDtos.Length == 0)
            throw new BadRequestException("At least one assignment is required.");

        _ = await vizsgaremekContext.Rendeleses.SingleOrDefaultAsync(x => x.Id == rendelesId)
            ?? throw new NotFoundException(nameof(Rendeles), rendelesId);

        var kolcsonzesReadDtos = new KolcsonzesReadDto[kolcsonzesWriteDtos.Length];
        for (int i = 0; i < kolcsonzesWriteDtos.Length; i++)
        {
            var dto = kolcsonzesWriteDtos[i]
                ?? throw new BadRequestException($"Assignment at index {i} must not be null.");

            Kolcsonzes kolcsonzes = mapper.Map<Kolcsonzes>(dto);

            Peldany peldany = await vizsgaremekContext.Peldanys.SingleOrDefaultAsync(x => x.Id == kolcsonzes.PeldanyId)
                ?? throw new NotFoundException(nameof(Peldany), kolcsonzes.PeldanyId);

            if (peldany.ActiveKolcsonzesId != null)
                throw new ConflictException($"Peldany '{peldany.Id}' is already assigned to an active kolcsonzes.");

            peldany.ActiveKolcsonzesId = kolcsonzes.Id;

            await vizsgaremekContext.Kolcsonzess.AddAsync(kolcsonzes);
            kolcsonzesReadDtos[i] = mapper.Map<KolcsonzesReadDto>(kolcsonzes);
        }

        await vizsgaremekContext.SaveChangesAsync();
        return kolcsonzesReadDtos;
    }
}