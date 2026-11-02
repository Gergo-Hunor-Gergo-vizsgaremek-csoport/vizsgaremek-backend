using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.Interfaces;

public interface IUserRoleService
{

    /// <summary>
    /// <para>Get current user</para>
    /// <para>When calling from a controler, pass the User property inherited from <see cref="ControllerBase"/></para>
    /// </summary>
    /// <param name="claimsPrincipal"><see cref="ControllerBase.User"/></param>
    /// <returns><see cref="User"/> object of current user</returns>
    /// <exception cref="NotLoggedInException">thrown when not logged in</exception>
    public Task<User> GetCurrentUserAsync(ClaimsPrincipal claimsPrincipal);
    public Task SetSysAdminRoleAsync(Guid user, bool set = true);

    public Task SetUserAdminRoleAsync(Guid user, bool set = true);

    public Task SetDeviceAdminRoleAsync(Guid user, bool set = true);

}