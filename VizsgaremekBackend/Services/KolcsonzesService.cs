using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using Kolcsonzes = VizsgaremekBackend.Models.Kolcsonzes;

namespace VizsgaremekBackend.Services;

public class KolcsonzesService(VizsgaremekContext vizsgaremekContext, IMapper mapper)
{
    public async Task<KolcsonzesReadDto> GetAsync(Guid id)
    {
        Kolcsonzes result = await vizsgaremekContext.Kolcsonzess.SingleAsync(x => x.Id == id);
        
        return mapper.Map<KolcsonzesReadDto>(result);
    }
    
    public async Task PostAsync(KolcsonzesReadDto dto)
    {
        Kolcsonzes newKolcsonzes = mapper.Map<Kolcsonzes>(dto);
        
        await vizsgaremekContext.Kolcsonzess.AddAsync(newKolcsonzes);
        
        await vizsgaremekContext.SaveChangesAsync();
    }
    
    public async Task PutAsync(Guid id, KolcsonzesWriteDto dto)
    {
        Kolcsonzes oldKolcsonzes = await vizsgaremekContext.Kolcsonzess.SingleAsync(x => x.Id == id);
        
        mapper.Map(dto, oldKolcsonzes);
        
        await vizsgaremekContext.SaveChangesAsync();
        
        
        
    }
    
    public async Task DeleteAsync(Guid id)
    {
        Kolcsonzes kolcsonzes = await vizsgaremekContext.Kolcsonzess.SingleAsync(x => x.Id == id);
        
        vizsgaremekContext.Kolcsonzess.Remove(kolcsonzes);
        
        await vizsgaremekContext.SaveChangesAsync();
    }
}