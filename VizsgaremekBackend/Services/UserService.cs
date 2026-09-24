using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Models;
using Type = System.Type;

namespace VizsgaremekBackend.Services;

public class UserService(VizsgaremekContext vizsgaremekContext, IMapper mapper)
{
    public async Task<List<UserReadDto>> SearchAsync(string query, int limit, int offset)
    {
        return ( await vizsgaremekContext.Users
                .Where(x => x.Name.Contains(query))
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
        
        await vizsgaremekContext.Users.AddAsync(newUser);
        
        await vizsgaremekContext.SaveChangesAsync();
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
        
        vizsgaremekContext.Users.Remove(user);
        
        await vizsgaremekContext.SaveChangesAsync();
    }
}