using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.Services;

public class RendelesService(VizsgaremekContext vizsgaremekContext, IMapper mapper)
{
    public async Task<RendelesReadDto> GetAsync(Guid id)
    {
        Rendeles result = await vizsgaremekContext.Rendeleses.SingleAsync(x=>x.Id == id);
        return mapper.Map<RendelesReadDto>(result);
    }
    
    public async Task<RendelesReadDto> PostAsync(RendelesWriteDto dto)
    {
        Rendeles newRendeles = mapper.Map<Rendeles>(dto);
        await vizsgaremekContext.Rendeleses.AddAsync(newRendeles);
        await vizsgaremekContext.SaveChangesAsync();
        return mapper.Map<RendelesReadDto>(newRendeles);
    }
    
    
    public async Task PutAsync(Guid id, RendelesWriteDto dto)
    {
        Rendeles oldRendeles = await vizsgaremekContext.Rendeleses.SingleAsync(x => x.Id == id);
        
        mapper.Map(dto, oldRendeles);
        
        await vizsgaremekContext.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(Guid id)
    {
        Rendeles rendeles = await vizsgaremekContext.Rendeleses.SingleAsync(x => x.Id == id);
        
        vizsgaremekContext.Rendeleses.Remove(rendeles);
        
        await vizsgaremekContext.SaveChangesAsync();
    }
}