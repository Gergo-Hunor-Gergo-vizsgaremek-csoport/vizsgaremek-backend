using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using Type = VizsgaremekBackend.Models.Type;

namespace VizsgaremekBackend.Services;

public class TypeService(VizsgaremekContext vizsgaremekContext, IMapper mapper)
{
    public async Task<List<TypeReadDto>> SearchAsync(string query, int limit, int offset)
    {
        if (limit < 0) throw new BadRequestException("Limit must be greater than or equal to 0.");
        if (offset < 0) throw new BadRequestException("Offset must be greater than or equal to 0.");

        query ??= string.Empty;

        return (await vizsgaremekContext.Types
                .Where(x => x.Name.ToLower().Contains(query.ToLower()))
                .Skip(offset)
                .Take(limit)
                .ToArrayAsync())
            .Select(mapper.Map<TypeReadDto>)
            .ToList();
    }

    public async Task<TypeReadDto> GetAsync(Guid id)
    {
        Type result = await vizsgaremekContext.Types.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Type), id);

        return mapper.Map<TypeReadDto>(result);
    }

    public async Task<TypeReadDto> PostAsync(TypeWriteDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Type newType = mapper.Map<Type>(dto);

        await vizsgaremekContext.Types.AddAsync(newType);
        await vizsgaremekContext.SaveChangesAsync();

        return mapper.Map<TypeReadDto>(newType);
    }

    public async Task PutAsync(Guid id, TypeWriteDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Type oldType = await vizsgaremekContext.Types.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Type), id);

        mapper.Map(dto, oldType);
        await vizsgaremekContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        Type type = await vizsgaremekContext.Types.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Type), id);

        vizsgaremekContext.Types.Remove(type);
        await vizsgaremekContext.SaveChangesAsync();
    }

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