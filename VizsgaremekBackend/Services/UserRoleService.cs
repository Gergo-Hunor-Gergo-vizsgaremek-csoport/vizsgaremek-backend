using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.Interfaces;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.Services;

public class UserRoleService(UserManager<User> userManager) : IUserRoleService
{
    /// <inheritdoc />
    public async Task<User> GetCurrentUserAsync(ClaimsPrincipal claimsPrincipal)
    {
        User user = (await userManager.GetUserAsync(claimsPrincipal))
                    ?? throw new NotLoggedInException();

        return user;
    }

    public async Task SetRoleAsync(Guid userId, string role, bool set = true)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new BadRequestException("Role must not be empty.");

        User user = await userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException(nameof(User), userId);

        IdentityResult result = set
            ? await userManager.AddToRoleAsync(user, role)
            : await userManager.RemoveFromRoleAsync(user, role);

        if (!result.Succeeded)
        {
            var errors = result.Errors.ToArray();
            var message = string.Join(", ", errors.Select(x => x.Description));

            if (errors.Any(x => x.Code is "UserAlreadyInRole" or "UserNotInRole"))
                throw new ConflictException(message);

            throw new BadRequestException(message);
        }
    }

    public async Task SetSysAdminRoleAsync(Guid user, bool set = true)
        => await SetRoleAsync(user, "SysAdmin", set);

    public async Task SetUserAdminRoleAsync(Guid user, bool set = true)
        => await SetRoleAsync(user, "UserAdmin", set);

    public async Task SetDeviceAdminRoleAsync(Guid user, bool set = true)
        => await SetRoleAsync(user, "DeviceAdmin", set);
}