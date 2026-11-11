using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.Interfaces;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.Services;

public class PeldanyService(VizsgaremekContext vizsgaremekContext, IMapper mapper)
    : IPeldanyService
{
    /// <inheritdoc />
    public async Task<PeldanyReadDto> GetAsync(Guid id)
    {
        Peldany result = await vizsgaremekContext.Peldanys.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Peldany), id);

        return mapper.Map<PeldanyReadDto>(result);
    }

    /// <inheritdoc />
    public async Task<PeldanyReadDto> PostAsync(PeldanyWriteDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Peldany newPeldany = mapper.Map<Peldany>(dto);

        await vizsgaremekContext.Peldanys.AddAsync(newPeldany);
        await vizsgaremekContext.SaveChangesAsync();

        return mapper.Map<PeldanyReadDto>(newPeldany);
    }

    /// <inheritdoc />
    public async Task PutAsync(Guid id, PeldanyWriteDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Peldany oldPeldany = await vizsgaremekContext.Peldanys.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Peldany), id);

        mapper.Map(dto, oldPeldany);
        await vizsgaremekContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        Peldany peldany = await vizsgaremekContext.Peldanys.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Peldany), id);

        vizsgaremekContext.Peldanys.Remove(peldany);
        await vizsgaremekContext.SaveChangesAsync();
    }
}