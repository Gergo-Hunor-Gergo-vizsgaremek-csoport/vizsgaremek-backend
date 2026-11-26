using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.Interfaces;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.Services;

public class RendelesService(VizsgaremekContext vizsgaremekContext, IMapper mapper)
    : IRendelesService
{
    /// <inheritdoc />
    public async Task<RendelesReadDto> GetAsync(Guid id)
    {
        Rendeles result = await vizsgaremekContext.Rendeleses.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Rendeles), id);

        return mapper.Map<RendelesReadDto>(result);
    }

    /// <inheritdoc />
    public async Task<RendelesReadDto> PostAsync(RendelesWriteDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Rendeles newRendeles = mapper.Map<Rendeles>(dto);
        await vizsgaremekContext.Rendeleses.AddAsync(newRendeles);
        await vizsgaremekContext.SaveChangesAsync();
        return mapper.Map<RendelesReadDto>(newRendeles);
    }

    /// <inheritdoc />
    public async Task PutAsync(Guid id, RendelesWriteDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Rendeles oldRendeles = await vizsgaremekContext.Rendeleses.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Rendeles), id);

        mapper.Map(dto, oldRendeles);
        await vizsgaremekContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        Rendeles rendeles = await vizsgaremekContext.Rendeleses.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Rendeles), id);

        vizsgaremekContext.Rendeleses.Remove(rendeles);
        await vizsgaremekContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<KolcsonzesReadDto> AssignPeldanyAsync(Guid rendelesId, KolcsonzesWriteDto kolcsonzesWriteDto)
    {
        Rendeles rendeles = await vizsgaremekContext.Rendeleses.SingleOrDefaultAsync(x => x.Id == rendelesId)
                          ?? throw new NotFoundException(nameof(Rendeles), rendelesId);

        ArgumentNullException.ThrowIfNull(kolcsonzesWriteDto);
        
        if (rendeles.CompletedQuantity >= rendeles.Quantity)
        {
            throw new BadRequestException("Order already has enough items assigned to it");
        }


        _ = await vizsgaremekContext.Rendeleses.SingleOrDefaultAsync(x => x.Id == rendelesId)
            ?? throw new NotFoundException(nameof(Rendeles), rendelesId);

        Kolcsonzes kolcsonzes = mapper.Map<Kolcsonzes>(kolcsonzesWriteDto);

        Peldany peldany = await vizsgaremekContext.Peldanys.SingleOrDefaultAsync(x => x.Id == kolcsonzes.PeldanyId)
        ?? throw new NotFoundException(nameof(Peldany), kolcsonzes.PeldanyId);

        if (peldany.ActiveKolcsonzesId != null) 
            throw new ConflictException($"Peldany '{peldany.Id}' is already assigned to an active kolcsonzes.");

        peldany.ActiveKolcsonzesId = kolcsonzes.Id;

        await vizsgaremekContext.Kolcsonzess.AddAsync(kolcsonzes);
        KolcsonzesReadDto kolcsonzesReadDto =  mapper.Map<KolcsonzesReadDto>(kolcsonzes);

        rendeles.CompletedQuantity++;

        await vizsgaremekContext.SaveChangesAsync();
        return kolcsonzesReadDto;
    }
}