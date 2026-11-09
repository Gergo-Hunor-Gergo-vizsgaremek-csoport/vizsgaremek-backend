using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
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
        ArgumentNullException.ThrowIfNull(query);
        if (limit < 0) throw new BadRequestException("Limit must be greater than or equal to 0.");
        if (offset < 0) throw new BadRequestException("Offset must be greater than or equal to 0.");

        var normalizedQuery = query.ToLower();

        return (await vizsgaremekContext.Users
                .Where(x => x.UserName != null && x.UserName.ToLower().Contains(normalizedQuery))
                .Skip(offset)
                .Take(limit)
                .ToArrayAsync())
            .Select(mapper.Map<UserReadDto>)
            .ToList();
    }

    public async Task<UserReadDto> GetAsync(Guid id)
    {
        User result = await vizsgaremekContext.Users.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(User), id);

        return mapper.Map<UserReadDto>(result);
    }

    public async Task<UserReadDto> PostAsync(UserWriteDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (string.IsNullOrWhiteSpace(dto.Password))
            throw new BadRequestException("Password is required when creating a new user.");

        User newUser = mapper.Map<User>(dto);

        IdentityResult result = await userManager.CreateAsync(newUser, dto.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.ToArray();
            var message = string.Join(", ", errors.Select(x => x.Description));
            if (errors.Any(x => x.Code is "DuplicateUserName" or "DuplicateEmail"))
                throw new ConflictException(message);
            throw new BadRequestException(message);
        }

        return mapper.Map<UserReadDto>(newUser);
    }

    public async Task PutAsync(Guid id, UserWriteDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        User oldUser = await vizsgaremekContext.Users.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(User), id);

        mapper.Map(dto, oldUser);
        await vizsgaremekContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        User user = await userManager.FindByIdAsync(id.ToString())
            ?? throw new NotFoundException(nameof(User), id);

        IdentityResult result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
            throw new BadRequestException(string.Join(", ", result.Errors.Select(x => x.Description)));
    }

    public async Task<KolcsonzesReadDto[]> GetKolcsonzesekAsync(Guid id)
    {
        User? user = await vizsgaremekContext.Users
            .Include(x => x.Kolcsonzesek)
            .SingleOrDefaultAsync(x => x.Id == id);

        if (user is null) throw new NotFoundException(nameof(User), id);

        return user.Kolcsonzesek
            .Select(mapper.Map<KolcsonzesReadDto>)
            .ToArray();
    }

    public async Task<RendelesReadDto[]> GetRendelesekAsync(Guid id)
    {
        User? user = await vizsgaremekContext.Users
            .Include(user => user.Rendeleses)
            .SingleOrDefaultAsync(x => x.Id == id);

        if (user is null) throw new NotFoundException(nameof(User), id);

        return user.Rendeleses
            .Select(x => mapper.Map<RendelesReadDto>(x))
            .ToArray();
    }

    public async Task<PeldanyReadDto[]> GetFelelossegekAsync(Guid id)
    {
        User? user = await vizsgaremekContext.Users
            .Include(x => x.Peldanys)
            .SingleOrDefaultAsync(x => x.Id == id);

        if (user is null) throw new NotFoundException(nameof(User), id);

        return user.Peldanys
            .Select(x => mapper.Map<PeldanyReadDto>(x))
            .ToArray();
    }
}