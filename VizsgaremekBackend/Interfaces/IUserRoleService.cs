using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.Interfaces;

public interface IUserRoleService
{
    /// <summary>
    /// Retrieves the administrative role memberships of the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose roles are to be checked.</param>
    /// <returns>
    /// A tuple of three flags indicating whether the user is in each administrative role:
    /// <list type="bullet">
    /// <item><description><c>sysAdmin</c>: the user is in the <c>SysAdmin</c> role.</description></item>
    /// <item><description><c>userAdmin</c>: the user is in the <c>UserAdmin</c> role.</description></item>
    /// <item><description><c>deviceAdmin</c>: the user is in the <c>DeviceAdmin</c> role.</description></item>
    /// </list>
    /// </returns>
    /// <exception cref="NotFoundException">
    /// Thrown when no user with the given <paramref name="userId"/> exists.
    /// </exception>
    public Task<(bool sysAdmin, bool userAdmin, bool deviceAdmin)> GetUserRolesAsync(Guid userId);
    
    /// <summary>
    /// Resolves the currently authenticated user from the provided claims principal.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal representing the current request user (for example, <see cref="ControllerBase.User"/> in a controller).</param>
    /// <returns>The resolved <see cref="User"/> entity.</returns>
    /// <exception cref="NotLoggedInException">Thrown when no user can be resolved from <paramref name="claimsPrincipal"/>.</exception>
    public Task<User> GetCurrentUserAsync(ClaimsPrincipal claimsPrincipal);

    /// <summary>
    /// Adds or removes the <c>SysAdmin</c> role for a user.
    /// </summary>
    /// <param name="user">The unique identifier of the target user.</param>
    /// <param name="set"><c>true</c> to add the role; <c>false</c> to remove it. Defaults to <c>true</c>.</param>
    /// <returns>A task that represents the asynchronous role update operation.</returns>
    /// <exception cref="BadRequestException">Thrown when the role operation fails with a non-conflict error.</exception>
    /// <exception cref="NotFoundException">Thrown when no user exists with the specified <paramref name="user"/>.</exception>
    /// <exception cref="ConflictException">Thrown when the user already has the role or does not have it when removal is requested.</exception>
    public Task SetSysAdminRoleAsync(Guid user, bool set = true);

    /// <summary>
    /// Adds or removes the <c>UserAdmin</c> role for a user.
    /// </summary>
    /// <param name="user">The unique identifier of the target user.</param>
    /// <param name="set"><c>true</c> to add the role; <c>false</c> to remove it. Defaults to <c>true</c>.</param>
    /// <returns>A task that represents the asynchronous role update operation.</returns>
    /// <exception cref="BadRequestException">Thrown when the role operation fails with a non-conflict error.</exception>
    /// <exception cref="NotFoundException">Thrown when no user exists with the specified <paramref name="user"/>.</exception>
    /// <exception cref="ConflictException">Thrown when the user already has the role or does not have it when removal is requested.</exception>
    public Task SetUserAdminRoleAsync(Guid user, bool set = true);

    /// <summary>
    /// Adds or removes the <c>DeviceAdmin</c> role for a user.
    /// </summary>
    /// <param name="user">The unique identifier of the target user.</param>
    /// <param name="set"><c>true</c> to add the role; <c>false</c> to remove it. Defaults to <c>true</c>.</param>
    /// <returns>A task that represents the asynchronous role update operation.</returns>
    /// <exception cref="BadRequestException">Thrown when the role operation fails with a non-conflict error.</exception>
    /// <exception cref="NotFoundException">Thrown when no user exists with the specified <paramref name="user"/>.</exception>
    /// <exception cref="ConflictException">Thrown when the user already has the role or does not have it when removal is requested.</exception>
    public Task SetDeviceAdminRoleAsync(Guid user, bool set = true);
}