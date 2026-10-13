using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.Interfaces;

public interface IUserRoleService
{

    public Task SetSysAdminRoleAsync(Guid user, bool set = true);

    public Task SetUserAdminRoleAsync(Guid user, bool set = true);

    public Task SetDeviceAdminRoleAsync(Guid user, bool set = true);

}