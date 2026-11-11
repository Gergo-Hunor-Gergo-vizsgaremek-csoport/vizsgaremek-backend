using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.IntegrationTests.Infrastructure;
using VizsgaremekBackend.Interfaces;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.IntegrationTests.Features.DbAccess;

public class UserRoleServiceTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    private async Task<User> CreateUserAsync(UserManager<User> userManager)
    {
        var user = new User { Id = Guid.NewGuid(), UserName = $"role_{Guid.NewGuid():N}", Email = $"role_{Guid.NewGuid():N}@example.com" };
        var res = await userManager.CreateAsync(user, "Password1");
        res.Succeeded.Should().BeTrue();
        return user;
    }

    [Fact]
    public async Task SetSysAdminRoleAsync_WhenSetTrue_ShouldAddRole()
    {
        using var scope = fixture.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleService = scope.ServiceProvider.GetRequiredService<IUserRoleService>();
        var user = await CreateUserAsync(userManager);

        await roleService.SetSysAdminRoleAsync(user.Id, true);

        (await userManager.IsInRoleAsync(user, "SysAdmin")).Should().BeTrue();
    }

    [Fact]
    public async Task SetSysAdminRoleAsync_WhenSetFalse_ShouldRemoveRole()
    {
        using var scope = fixture.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleService = scope.ServiceProvider.GetRequiredService<IUserRoleService>();
        var user = await CreateUserAsync(userManager);
        await roleService.SetSysAdminRoleAsync(user.Id, true);

        await roleService.SetSysAdminRoleAsync(user.Id, false);

        (await userManager.IsInRoleAsync(user, "SysAdmin")).Should().BeFalse();
    }

    [Fact]
    public async Task SetUserAdminRoleAsync_WhenSetTrue_ShouldAddRole()
    {
        using var scope = fixture.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleService = scope.ServiceProvider.GetRequiredService<IUserRoleService>();
        var user = await CreateUserAsync(userManager);
        await roleService.SetUserAdminRoleAsync(user.Id, true);
        (await userManager.IsInRoleAsync(user, "UserAdmin")).Should().BeTrue();
    }

    [Fact]
    public async Task SetDeviceAdminRoleAsync_WhenSetTrue_ShouldAddRole()
    {
        using var scope = fixture.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleService = scope.ServiceProvider.GetRequiredService<IUserRoleService>();
        var user = await CreateUserAsync(userManager);
        await roleService.SetDeviceAdminRoleAsync(user.Id, true);
        (await userManager.IsInRoleAsync(user, "DeviceAdmin")).Should().BeTrue();
    }

    [Fact]
    public async Task SetSysAdminRoleAsync_WhenUserMissing_ShouldThrowNotFoundException()
    {
        using var scope = fixture.CreateScope();
        var roleService = scope.ServiceProvider.GetRequiredService<IUserRoleService>();

        Func<Task> act = async () => await roleService.SetSysAdminRoleAsync(Guid.NewGuid(), true);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task SetSysAdminRoleAsync_WhenAddingDuplicateRole_ShouldThrowConflictException()
    {
        using var scope = fixture.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleService = scope.ServiceProvider.GetRequiredService<IUserRoleService>();
        var user = await CreateUserAsync(userManager);
        await roleService.SetSysAdminRoleAsync(user.Id, true);

        Func<Task> act = async () => await roleService.SetSysAdminRoleAsync(user.Id, true);

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task GetCurrentUserAsync_WhenNotResolved_ShouldThrowNotLoggedInException()
    {
        using var scope = fixture.CreateScope();
        var roleService = scope.ServiceProvider.GetRequiredService<IUserRoleService>();

        Func<Task> act = async () => await roleService.GetCurrentUserAsync(new ClaimsPrincipal(new ClaimsIdentity()));

        await act.Should().ThrowAsync<NotLoggedInException>();
    }
}
