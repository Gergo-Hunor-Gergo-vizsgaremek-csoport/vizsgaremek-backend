using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.Services;

public class PeldanyService(VizsgaremekContext vizsgaremekContext, IMapper mapper)
{
    public async Task<PeldanyReadDto> GetAsync(Guid id)
    {
        Peldany result = await vizsgaremekContext.Peldanys.SingleAsync(x => x.Id == id);
        
        return mapper.Map<PeldanyReadDto>(result);
    }
    
    public async Task PostAsync(PeldanyWriteDto dto)
    {
        Peldany newPeldany = mapper.Map<Peldany>(dto);
        
        await vizsgaremekContext.Peldanys.AddAsync(newPeldany);
        
        await vizsgaremekContext.SaveChangesAsync();
    }
    
    public async Task PutAsync(Guid id, PeldanyWriteDto dto)
    {
        Peldany oldPeldany = await vizsgaremekContext.Peldanys.SingleAsync(x => x.Id == id);
        
        mapper.Map(dto, oldPeldany);
        
        await vizsgaremekContext.SaveChangesAsync();
        
        
        
    }
    
    public async Task DeleteAsync(Guid id)
    {
        Peldany peldany = await vizsgaremekContext.Peldanys.SingleAsync(x => x.Id == id);
        
        vizsgaremekContext.Peldanys.Remove(peldany);
        
        await vizsgaremekContext.SaveChangesAsync();
    }
}