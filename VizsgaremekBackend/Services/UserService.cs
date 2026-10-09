using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Interfaces;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.Services;

public class UserService(
    VizsgaremekContext vizsgaremekContext,
    IMapper mapper,
    UserManager<User> userManager) 
    : IUserService
{
    public async Task<List<UserReadDto>> SearchAsync(string query, int limit, int offset)
    {
        return ( await vizsgaremekContext.Users
                .Where(x => x.UserName.Contains(query))
                .Skip(offset)
                .Take(limit)
                .ToArrayAsync())
            .Select(mapper.Map<UserReadDto>)
            .ToList();
    }

    public async Task<UserReadDto> GetAsync(Guid id)
    {
        User result = await vizsgaremekContext.Users.SingleAsync(x => x.Id == id);
        
        return mapper.Map<UserReadDto>(result);
    }
    
    public async Task PostAsync(UserWriteDto dto)
    {
        User newUser = mapper.Map<User>(dto);
        
        await userManager
            .CreateAsync(newUser,
                dto.Password 
                ?? throw new ArgumentException("Password is required when creating a new user"));
        
        //await vizsgaremekContext.Users.AddAsync(newUser);
        
        //await vizsgaremekContext.SaveChangesAsync();
    }
    
    public async Task PutAsync(Guid id, UserWriteDto dto)
    {
        User oldUser = await vizsgaremekContext.Users.SingleAsync(x => x.Id == id);
        
        mapper.Map(dto, oldUser);
        
        await vizsgaremekContext.SaveChangesAsync();
        
    }
    
    public async Task DeleteAsync(Guid id)
    {
        User user = await vizsgaremekContext.Users.SingleAsync(x => x.Id == id);
        
        await userManager.DeleteAsync(user);
        
        //vizsgaremekContext.Users.Remove(user);
        
        //await vizsgaremekContext.SaveChangesAsync();
    }
    
    public async Task<KolcsonzesReadDto[]> GetKolcsonzesekAsync(Guid id)
    {
        return (vizsgaremekContext.Users
            .Include(x => x.Kolcsonzesek)
            .Single(x => x.Id == id)
            .Kolcsonzesek
            .Select(mapper.Map<KolcsonzesReadDto>).ToArray());
    }
    
    public async Task<RendelesReadDto[]> GetRendelesekAsync(Guid id)
    {
        return (vizsgaremekContext.Users
                .Include(user => user.Rendeleses)
                .Single(x => x.Id == id)
                .Rendeleses
                .Select(x => mapper.Map<RendelesReadDto>(x))
            ).ToArray();
    }

    public async Task<PeldanyReadDto[]> GetFelelossegekAsync(Guid id)
    {
        return (vizsgaremekContext.Users
                .Include(x => x.Peldanys)
                .Single(x => x.Id == id)
                .Peldanys
                .Select(x => mapper.Map<PeldanyReadDto>(x))
            ).ToArray();
    }
}