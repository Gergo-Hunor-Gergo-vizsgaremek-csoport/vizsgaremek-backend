using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.Services;

public class LogService(VizsgaremekContext vizsgaremekContext, IMapper mapper)
{
    public async Task<List<LogReadDto>> SearchAsync(string query, int limit, int offset)
    {
        if (limit < 0) throw new BadRequestException("Limit must be greater than or equal to 0.");
        if (offset < 0) throw new BadRequestException("Offset must be greater than or equal to 0.");

        query ??= string.Empty;

        return (await vizsgaremekContext.Logs
                .Where(x => x.Type.ToLower().Contains(query.ToLower()))
                .Skip(offset)
                .Take(limit)
                .ToArrayAsync())
            .Select(mapper.Map<LogReadDto>)
            .ToList();
    }

    public async Task<LogReadDto> GetAsync(Guid id)
    {
        Log result = await vizsgaremekContext.Logs.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Log), id);

        return mapper.Map<LogReadDto>(result);
    }

    public async Task<LogReadDto> PostAsync(LogWriteDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Log newLog = mapper.Map<Log>(dto);

        await vizsgaremekContext.Logs.AddAsync(newLog);
        await vizsgaremekContext.SaveChangesAsync();

        return mapper.Map<LogReadDto>(newLog);
    }

    public async Task PutAsync(Guid id, LogWriteDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Log oldLog = await vizsgaremekContext.Logs.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Log), id);

        mapper.Map(dto, oldLog);
        await vizsgaremekContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        Log log = await vizsgaremekContext.Logs.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Log), id);

        vizsgaremekContext.Logs.Remove(log);
        await vizsgaremekContext.SaveChangesAsync();
    }
}