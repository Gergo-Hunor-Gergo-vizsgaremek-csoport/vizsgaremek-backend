using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.IntegrationTests.Infrastructure;
using VizsgaremekBackend.Interfaces;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.IntegrationTests.Features.DbAccess;

public class UserServiceTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    private static IUserService GetService(IServiceScope scope) => scope.ServiceProvider.GetRequiredService<IUserService>();

    [Fact]
    public async Task SearchAsync_ShouldReturnOnlyMatchingUsers()
    {
        using var scope = fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var service = GetService(scope);
        var match = new User { Id = Guid.NewGuid(), UserName = "match_user_abc", Email = "m1@example.com" };
        var other = new User { Id = Guid.NewGuid(), UserName = "other_user_xyz", Email = "m2@example.com" };
        db.Users.AddRange(match, other);
        await db.SaveChangesAsync();

        var result = await service.SearchAsync("match_user_abc", 10, 0);

        result.Should().Contain(x => x.Id == match.Id);
        result.Should().NotContain(x => x.Id == other.Id);
        result.Should().OnlyContain(x => x.Username.Contains("match_user_abc", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task SearchAsync_WithNullQuery_ShouldThrowArgumentNullException()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);
        Func<Task> act = async () => await service.SearchAsync(null!, 10, 0);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task SearchAsync_WithNegativeLimit_ShouldThrowBadRequestException()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);
        Func<Task> act = async () => await service.SearchAsync("x", -1, 0);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task SearchAsync_WithNegativeOffset_ShouldThrowBadRequestException()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);
        Func<Task> act = async () => await service.SearchAsync("x", 10, -1);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task PostAsync_WithValidDto_ShouldCreateUser()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);
        var dto = new UserWriteDto { UserName = $"u_{Guid.NewGuid():N}", Email = $"u_{Guid.NewGuid():N}@example.com", Password = "Password1" };

        var created = await service.PostAsync(dto);

        created.Id.Should().NotBeEmpty();
        created.Username.Should().Be(dto.UserName);
    }

    [Fact]
    public async Task PostAsync_WithNullDto_ShouldThrowArgumentNullException()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);
        Func<Task> act = async () => await service.PostAsync(null!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task PostAsync_WithMissingPassword_ShouldThrowBadRequestException()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);
        var dto = new UserWriteDto { UserName = $"u_{Guid.NewGuid():N}", Email = $"u_{Guid.NewGuid():N}@example.com", Password = null };
        Func<Task> act = async () => await service.PostAsync(dto);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task GetAsync_WhenExists_ShouldReturnUser()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);
        var dto = new UserWriteDto { UserName = $"g_{Guid.NewGuid():N}", Email = $"g_{Guid.NewGuid():N}@example.com", Password = "Password1" };
        var created = await service.PostAsync(dto);

        var fetched = await service.GetAsync(created.Id);

        fetched.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task GetAsync_WhenMissing_ShouldThrowNotFoundException()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);
        Func<Task> act = async () => await service.GetAsync(Guid.NewGuid());
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task PutAsync_WhenExists_ShouldUpdateUser()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);
        var dto = new UserWriteDto { UserName = $"p_{Guid.NewGuid():N}", Email = $"p_{Guid.NewGuid():N}@example.com", Password = "Password1" };
        var created = await service.PostAsync(dto);
        var update = new UserWriteDto { UserName = dto.UserName + "_u", Email = dto.Email, Password = null };

        await service.PutAsync(created.Id, update);

        var updated = await service.GetAsync(created.Id);
        updated.Username.Should().Be(update.UserName);
    }

    [Fact]
    public async Task PutAsync_WithNullDto_ShouldThrowArgumentNullException()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);
        Func<Task> act = async () => await service.PutAsync(Guid.NewGuid(), null!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task PutAsync_WhenMissing_ShouldThrowNotFoundException()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);
        Func<Task> act = async () => await service.PutAsync(Guid.NewGuid(), new UserWriteDto { UserName = "x", Email = "x@example.com" });
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenExists_ShouldDeleteUser()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);
        var created = await service.PostAsync(new UserWriteDto { UserName = $"d_{Guid.NewGuid():N}", Email = $"d_{Guid.NewGuid():N}@example.com", Password = "Password1" });

        await service.DeleteAsync(created.Id);

        Func<Task> act = async () => await service.GetAsync(created.Id);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenMissing_ShouldThrowNotFoundException()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);
        Func<Task> act = async () => await service.DeleteAsync(Guid.NewGuid());
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetKolcsonzesekAsync_WhenUserExists_ShouldReturnOnlyUsersKolcsonzesek()
    {
        using var scope = fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var service = GetService(scope);
        var u1 = await service.PostAsync(new UserWriteDto { UserName = $"k1_{Guid.NewGuid():N}", Email = $"k1_{Guid.NewGuid():N}@example.com", Password = "Password1" });
        var u2 = await service.PostAsync(new UserWriteDto { UserName = $"k2_{Guid.NewGuid():N}", Email = $"k2_{Guid.NewGuid():N}@example.com", Password = "Password1" });
        var type = new VizsgaremekBackend.Models.Type { Id = Guid.NewGuid(), Name = "T", Description = "D", Icon = "i" };
        var location = new Location { Id = Guid.NewGuid(), Name = "L" };
        db.Types.Add(type); db.Locations.Add(location);
        await db.SaveChangesAsync();
        var p1 = new Peldany { Id = Guid.NewGuid(), TypeId = type.Id, FelelosId = u1.Id, LocationId = location.Id, AddDate = DateTime.UtcNow, SerialNumber = $"S-{Guid.NewGuid():N}", IsHibas = false, IsSelejt = false, IsSelejtSugg = false };
        var p2 = new Peldany { Id = Guid.NewGuid(), TypeId = type.Id, FelelosId = u2.Id, LocationId = location.Id, AddDate = DateTime.UtcNow, SerialNumber = $"S-{Guid.NewGuid():N}", IsHibas = false, IsSelejt = false, IsSelejtSugg = false };
        db.Peldanys.AddRange(p1, p2);
        await db.SaveChangesAsync();
        var k1 = new Kolcsonzes { Id = Guid.NewGuid(), UserId = u1.Id, PeldanyId = p1.Id, Date = DateTime.UtcNow };
        var k2 = new Kolcsonzes { Id = Guid.NewGuid(), UserId = u2.Id, PeldanyId = p2.Id, Date = DateTime.UtcNow };
        db.Kolcsonzess.AddRange(k1, k2);
        await db.SaveChangesAsync();

        var result = await service.GetKolcsonzesekAsync(u1.Id);

        result.Should().Contain(x => x.Id == k1.Id);
        result.Should().NotContain(x => x.Id == k2.Id);
    }

    [Fact]
    public async Task GetKolcsonzesekAsync_WhenUserMissing_ShouldThrowNotFoundException()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);
        Func<Task> act = async () => await service.GetKolcsonzesekAsync(Guid.NewGuid());
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetRendelesekAsync_WhenUserExists_ShouldReturnOnlyUsersRendelesek()
    {
        using var scope = fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var service = GetService(scope);
        var u1 = await service.PostAsync(new UserWriteDto { UserName = $"r1_{Guid.NewGuid():N}", Email = $"r1_{Guid.NewGuid():N}@example.com", Password = "Password1" });
        var u2 = await service.PostAsync(new UserWriteDto { UserName = $"r2_{Guid.NewGuid():N}", Email = $"r2_{Guid.NewGuid():N}@example.com", Password = "Password1" });
        var type = new VizsgaremekBackend.Models.Type { Id = Guid.NewGuid(), Name = "T", Description = "D", Icon = "i" };
        db.Types.Add(type); await db.SaveChangesAsync();
        var r1 = new Rendeles { Id = Guid.NewGuid(), UserId = u1.Id, TypeId = type.Id, Date = DateTime.UtcNow, Quantity = 1, CompletedQuantity = 0 };
        var r2 = new Rendeles { Id = Guid.NewGuid(), UserId = u2.Id, TypeId = type.Id, Date = DateTime.UtcNow, Quantity = 1, CompletedQuantity = 0 };
        db.Rendeleses.AddRange(r1, r2); await db.SaveChangesAsync();

        var result = await service.GetRendelesekAsync(u1.Id);

        result.Should().Contain(x => x.Id == r1.Id);
        result.Should().NotContain(x => x.Id == r2.Id);
    }

    [Fact]
    public async Task GetRendelesekAsync_WhenUserMissing_ShouldThrowNotFoundException()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);
        Func<Task> act = async () => await service.GetRendelesekAsync(Guid.NewGuid());
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetFelelossegekAsync_WhenUserExists_ShouldReturnOnlyUsersFelelossegek()
    {
        using var scope = fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var service = GetService(scope);
        var u1 = await service.PostAsync(new UserWriteDto { UserName = $"f1_{Guid.NewGuid():N}", Email = $"f1_{Guid.NewGuid():N}@example.com", Password = "Password1" });
        var u2 = await service.PostAsync(new UserWriteDto { UserName = $"f2_{Guid.NewGuid():N}", Email = $"f2_{Guid.NewGuid():N}@example.com", Password = "Password1" });
        var type = new VizsgaremekBackend.Models.Type { Id = Guid.NewGuid(), Name = "T", Description = "D", Icon = "i" };
        var location = new Location { Id = Guid.NewGuid(), Name = "L" };
        db.Types.Add(type); db.Locations.Add(location); await db.SaveChangesAsync();
        var p1 = new Peldany { Id = Guid.NewGuid(), TypeId = type.Id, FelelosId = u1.Id, LocationId = location.Id, AddDate = DateTime.UtcNow, SerialNumber = $"S-{Guid.NewGuid():N}", IsHibas = false, IsSelejt = false, IsSelejtSugg = false };
        var p2 = new Peldany { Id = Guid.NewGuid(), TypeId = type.Id, FelelosId = u2.Id, LocationId = location.Id, AddDate = DateTime.UtcNow, SerialNumber = $"S-{Guid.NewGuid():N}", IsHibas = false, IsSelejt = false, IsSelejtSugg = false };
        db.Peldanys.AddRange(p1, p2); await db.SaveChangesAsync();

        var result = await service.GetFelelossegekAsync(u1.Id);

        result.Should().Contain(x => x.Id == p1.Id);
        result.Should().NotContain(x => x.Id == p2.Id);
    }

    [Fact]
    public async Task GetFelelossegekAsync_WhenUserMissing_ShouldThrowNotFoundException()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);
        Func<Task> act = async () => await service.GetFelelossegekAsync(Guid.NewGuid());
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
