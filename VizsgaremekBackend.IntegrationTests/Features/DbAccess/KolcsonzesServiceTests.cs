using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.IntegrationTests.Infrastructure;
using VizsgaremekBackend.Models;
using VizsgaremekBackend.Services;

namespace VizsgaremekBackend.IntegrationTests.Features.DbAccess;

public class KolcsonzesServiceTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    private static KolcsonzesService GetService(IServiceScope scope) => scope.ServiceProvider.GetRequiredService<KolcsonzesService>();

    private async Task<(User user, Peldany peldany)> SeedAsync(VizsgaremekContext db)
    {
        var user = new User { Id = Guid.NewGuid(), UserName = $"k_{Guid.NewGuid():N}", Email = $"k_{Guid.NewGuid():N}@example.com" };
        var type = new VizsgaremekBackend.Models.Type { Id = Guid.NewGuid(), Name = "T", Description = "D", Icon = "i" };
        var location = new Location { Id = Guid.NewGuid(), Name = "L" };
        db.Users.Add(user); db.Types.Add(type); db.Locations.Add(location);
        await db.SaveChangesAsync();
        var peldany = new Peldany { Id = Guid.NewGuid(), TypeId = type.Id, FelelosId = user.Id, LocationId = location.Id, AddDate = DateTime.UtcNow, SerialNumber = $"SN-{Guid.NewGuid():N}", IsHibas = false, IsSelejt = false, IsSelejtSugg = false };
        db.Peldanys.Add(peldany);
        await db.SaveChangesAsync();
        return (user, peldany);
    }

    [Fact]
    public async Task PostAsync_WithValidDto_ShouldCreateKolcsonzes()
    {
        using var scope = fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var service = GetService(scope);
        var (user, peldany) = await SeedAsync(db);

        var created = await service.PostAsync(new KolcsonzesWriteDto { UserId = user.Id, PeldanyId = peldany.Id, Date = DateTime.UtcNow });

        created.Id.Should().NotBeEmpty();
        created.PeldanyId.Should().Be(peldany.Id);
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
    public async Task GetAsync_WhenExists_ShouldReturnKolcsonzes()
    {
        using var scope = fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var service = GetService(scope);
        var (user, peldany) = await SeedAsync(db);
        var created = await service.PostAsync(new KolcsonzesWriteDto { UserId = user.Id, PeldanyId = peldany.Id, Date = DateTime.UtcNow });

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
    public async Task PutAsync_WhenExists_ShouldUpdateKolcsonzes()
    {
        using var scope = fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var service = GetService(scope);
        var (user, peldany) = await SeedAsync(db);
        var created = await service.PostAsync(new KolcsonzesWriteDto { UserId = user.Id, PeldanyId = peldany.Id, Date = DateTime.UtcNow });
        var newExp = DateTime.UtcNow.AddDays(5);

        await service.PutAsync(created.Id, new KolcsonzesWriteDto { UserId = user.Id, PeldanyId = peldany.Id, Date = created.Date, ExpirationDate = newExp });

        var updated = await service.GetAsync(created.Id);
        updated.ExpirationDate.Should().NotBeNull();
        (updated.ExpirationDate!.Value - newExp).Duration().Should().BeLessThanOrEqualTo(TimeSpan.FromSeconds(1));
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
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var service = GetService(scope);
        var (user, peldany) = await SeedAsync(db);
        Func<Task> act = async () => await service.PutAsync(Guid.NewGuid(), new KolcsonzesWriteDto { UserId = user.Id, PeldanyId = peldany.Id, Date = DateTime.UtcNow });
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenExists_ShouldDeleteKolcsonzes()
    {
        using var scope = fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var service = GetService(scope);
        var (user, peldany) = await SeedAsync(db);
        var created = await service.PostAsync(new KolcsonzesWriteDto { UserId = user.Id, PeldanyId = peldany.Id, Date = DateTime.UtcNow });

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
}
