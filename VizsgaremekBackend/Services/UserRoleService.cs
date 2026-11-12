using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.Interfaces;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.Services;

public class UserRoleService(UserManager<User> userManager) : IUserRoleService
{
    /// <inheritdoc />
    public async Task<(bool sysAdmin, bool userAdmin, bool deviceAdmin)> GetUserRolesAsync(Guid userId)
    {
        User user = await userManager.FindByIdAsync(userId.ToString()) 
                    ?? throw new NotFoundException(nameof(User), userId);
        
        bool sysAdmin = await userManager.IsInRoleAsync(user, "SysAdmin");
        bool userAdmin = await userManager.IsInRoleAsync(user, "UserAdmin");
        bool deviceAdmin = await userManager.IsInRoleAsync(user, "DeviceAdmin");
        
        return (sysAdmin, userAdmin, deviceAdmin);
    }

    /// <inheritdoc />
    public async Task<User> GetCurrentUserAsync(ClaimsPrincipal claimsPrincipal)
    {
        User user = (await userManager.GetUserAsync(claimsPrincipal))
                    ?? throw new NotLoggedInException();

        return user;
    }

    /// <summary>
    /// Adds or removes a named role for a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the target user.</param>
    /// <param name="role">The name of the role to add or remove.</param>
    /// <param name="set"><c>true</c> to add the role; <c>false</c> to remove it. Defaults to <c>true</c>.</param>
    /// <returns>A task that represents the asynchronous role update operation.</returns>
    /// <exception cref="BadRequestException">Thrown when <paramref name="role"/> is empty, or when the identity operation fails with a non-conflict error.</exception>
    /// <exception cref="NotFoundException">Thrown when no user exists with the specified <paramref name="userId"/>.</exception>
    /// <exception cref="ConflictException">Thrown when the user already has the role (<c>UserAlreadyInRole</c>) or does not have it when removal is requested (<c>UserNotInRole</c>).</exception>
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

    /// <inheritdoc />
    public async Task SetSysAdminRoleAsync(Guid user, bool set = true)
        => await SetRoleAsync(user, "SysAdmin", set);

    /// <inheritdoc />
    public async Task SetUserAdminRoleAsync(Guid user, bool set = true)
        => await SetRoleAsync(user, "UserAdmin", set);

    /// <inheritdoc />
    public async Task SetDeviceAdminRoleAsync(Guid user, bool set = true)
        => await SetRoleAsync(user, "DeviceAdmin", set);
}