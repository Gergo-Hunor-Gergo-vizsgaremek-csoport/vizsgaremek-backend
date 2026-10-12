using Microsoft.AspNetCore.Identity;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.Services;

public class UserRoleService(UserManager<User> userManager)
{
    
    public async Task SetRoleAsync(User user, string role, bool set = true)
    {
        await (set
            ? userManager.AddToRoleAsync(user, role)
            : userManager.RemoveFromRoleAsync(user, role));
    }
    
    public async Task SetSysAdminRoleAsync(User user, bool set = true) 
        => await SetRoleAsync(user, "SysAdmin", set);
    
    public async Task SetUserAdminRoleAsync(User user, bool set = true) 
        => await SetRoleAsync(user, "UserAdmin", set);
    
    public async Task SetDeviceAdminRoleAsync(User user, bool set = true) 
        => await SetRoleAsync(user, "DeviceAdmin", set);
    
}