using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using Type = VizsgaremekBackend.Models.Type;

namespace VizsgaremekBackend.Services;

public class TypeService(VizsgaremekContext vizsgaremekContext, IMapper mapper)
{
    public async Task<List<TypeReadDto>> SearchAsync(string query, int limit, int offset)
    {
        return ( await vizsgaremekContext.Types
            .Where(x => x.Name.Contains(query))
            .Skip(offset)
            .Take(limit)
            .ToArrayAsync())
            .Select(mapper.Map<TypeReadDto>)
            .ToList();
    }

    public async Task<TypeReadDto> GetAsync(Guid id)
    {
        Type result = await vizsgaremekContext.Types.SingleAsync(x => x.Id == id);
        
        return mapper.Map<TypeReadDto>(result);
    }
    
    public async Task PostAsync(TypeWriteDto dto)
    {
        Type newType = mapper.Map<Type>(dto);
        
        await vizsgaremekContext.Types.AddAsync(newType);
        
        await vizsgaremekContext.SaveChangesAsync();
    }
    
    public async Task PutAsync(Guid id, TypeWriteDto dto)
    {
        Type oldType = await vizsgaremekContext.Types.SingleAsync(x => x.Id == id);
        
        mapper.Map(dto, oldType);
        
        await vizsgaremekContext.SaveChangesAsync();
        
        
        
    }
    
    public async Task DeleteAsync(Guid id)
    {
        Type type = await vizsgaremekContext.Types.SingleAsync(x => x.Id == id);
        
        vizsgaremekContext.Types.Remove(type);
        
        await vizsgaremekContext.SaveChangesAsync();
    }
}