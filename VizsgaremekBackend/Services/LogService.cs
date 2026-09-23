using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.Services;

public class LogService(VizsgaremekContext vizsgaremekContext, IMapper mapper)
{
    public async Task<List<LogReadDto>> SearchAsync(string query, int limit, int offset)
    {
        return (await vizsgaremekContext.Logs
                .Where(x => x.Type.Contains(query))
                .Skip(offset)
                .Take(limit)
                .ToArrayAsync())
            .Select(mapper.Map<LogReadDto>)
            .ToList();
    }

    public async Task<LogReadDto> GetAsync(Guid id)
    {
        Log result = await vizsgaremekContext.Logs.SingleAsync(x => x.Id == id);
        return mapper.Map<LogReadDto>(result);
    }
    
    public async Task PostAsync(LogWriteDto dto)
    {
        Log newLog = mapper.Map<Log>(dto);
        
        await vizsgaremekContext.Logs.AddAsync(newLog);
        
        await vizsgaremekContext.SaveChangesAsync();
    }
    
    public async Task PutAsync(Guid id, LogWriteDto dto)
    {
        Log oldLog = await vizsgaremekContext.Logs.SingleAsync(x => x.Id == id);
        
        mapper.Map(dto, oldLog);
        
        await vizsgaremekContext.SaveChangesAsync();
        
        
        
    }
    
    public async Task DeleteAsync(Guid id)
    {
        Log log = await vizsgaremekContext.Logs.SingleAsync(x => x.Id == id);
        
        vizsgaremekContext.Logs.Remove(log);
        
        await vizsgaremekContext.SaveChangesAsync();
    }
}