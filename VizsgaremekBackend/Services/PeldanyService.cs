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
    
    public async Task<PeldanyReadDto[]> GetByTypeIdAsync(Guid typeId, int limit, int offset)
    {
        ArgumentNullException.ThrowIfNull(typeId);
        if (limit < 0) throw new BadRequestException("Limit must be greater than or equal to 0.");
        if (offset < 0) throw new BadRequestException("Offset must be greater than or equal to 0.");
        
        Peldany[] result = await vizsgaremekContext.Peldanys.Where(x => x.TypeId == typeId).OrderBy(x => x.TypeId)
            .Skip(offset).Take(limit).ToArrayAsync();
        
        return mapper.Map<PeldanyReadDto[]>(result);
    }
    
    public async Task<int> PeldanyCounterAsync(Guid id)
    {
        ArgumentNullException.ThrowIfNull(id);

        return await vizsgaremekContext.Peldanys
            .Where(x => x.TypeId == id)
            .CountAsync();
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

    public async Task<PeldanyReadDto[]> GetChildrenAsync(Guid id, int limit, int offset)
    {
        if (limit < 0) throw new BadRequestException("Limit must be greater than or equal to 0.");
        if (offset < 0) throw new BadRequestException("Offset must be greater than or equal to 0.");

        Peldany? peldany = await vizsgaremekContext.Peldanys
            .SingleOrDefaultAsync(x => x.Id == id);

        if (peldany is null) throw new NotFoundException(nameof(Peldany), id);

        return vizsgaremekContext.Peldanys
            .Where(x => x.ParentId == id)
            .Skip(offset)
            .Take(limit)
            .Select(x => mapper.Map<PeldanyReadDto>(x))
            .ToArray();
    }
}