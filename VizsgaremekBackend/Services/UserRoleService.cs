using Microsoft.AspNetCore.Identity;
using VizsgaremekBackend.Interfaces;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.Services;

public class UserRoleService(UserManager<User> userManager) : IUserRoleService
{
    
    public async Task SetRoleAsync(Guid userId, string role, bool set = true)
    {
        User user = await userManager.FindByIdAsync(userId.ToString());
        
        await (set
            ? userManager.AddToRoleAsync(user, role)
            : userManager.RemoveFromRoleAsync(user, role));
    }
    
    public async Task SetSysAdminRoleAsync(Guid user, bool set = true) 
        => await SetRoleAsync(user, "SysAdmin", set);
    
    public async Task SetUserAdminRoleAsync(Guid user, bool set = true) 
        => await SetRoleAsync(user, "UserAdmin", set);
    
    public async Task SetDeviceAdminRoleAsync(Guid user, bool set = true) 
        => await SetRoleAsync(user, "DeviceAdmin", set);
    
}