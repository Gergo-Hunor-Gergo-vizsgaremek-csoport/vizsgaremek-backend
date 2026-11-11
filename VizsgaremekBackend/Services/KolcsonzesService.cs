using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.Interfaces;
using Kolcsonzes = VizsgaremekBackend.Models.Kolcsonzes;

namespace VizsgaremekBackend.Services;

public class KolcsonzesService(VizsgaremekContext vizsgaremekContext, IMapper mapper)
    : IKolcsonzesService
{
    /// <inheritdoc />
    public async Task<KolcsonzesReadDto> GetAsync(Guid id)
    {
        Kolcsonzes result = await vizsgaremekContext.Kolcsonzess.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Kolcsonzes), id);

        return mapper.Map<KolcsonzesReadDto>(result);
    }

    /// <inheritdoc />
    public async Task<KolcsonzesReadDto> PostAsync(KolcsonzesWriteDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Kolcsonzes newKolcsonzes = mapper.Map<Kolcsonzes>(dto);

        await vizsgaremekContext.Kolcsonzess.AddAsync(newKolcsonzes);
        await vizsgaremekContext.SaveChangesAsync();

        return mapper.Map<KolcsonzesReadDto>(newKolcsonzes);
    }

    /// <inheritdoc />
    public async Task PutAsync(Guid id, KolcsonzesWriteDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Kolcsonzes oldKolcsonzes = await vizsgaremekContext.Kolcsonzess.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Kolcsonzes), id);

        mapper.Map(dto, oldKolcsonzes);
        await vizsgaremekContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        Kolcsonzes kolcsonzes = await vizsgaremekContext.Kolcsonzess.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Kolcsonzes), id);

        vizsgaremekContext.Kolcsonzess.Remove(kolcsonzes);
        await vizsgaremekContext.SaveChangesAsync();
    }
}