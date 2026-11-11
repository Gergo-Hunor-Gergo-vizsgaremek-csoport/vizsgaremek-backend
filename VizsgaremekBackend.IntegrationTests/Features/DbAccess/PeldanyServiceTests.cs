using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.IntegrationTests.Infrastructure;
using VizsgaremekBackend.Models;
using VizsgaremekBackend.Services;

namespace VizsgaremekBackend.IntegrationTests.Features.DbAccess;

public class PeldanyServiceTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    private static PeldanyService GetService(IServiceScope scope) => scope.ServiceProvider.GetRequiredService<PeldanyService>();

    private async Task<(User user, VizsgaremekBackend.Models.Type type, Location location)> SeedDepsAsync(VizsgaremekContext db)
    {
        var user = new User { Id = Guid.NewGuid(), UserName = $"p_{Guid.NewGuid():N}", Email = $"p_{Guid.NewGuid():N}@example.com" };
        var type = new VizsgaremekBackend.Models.Type { Id = Guid.NewGuid(), Name = "T", Description = "D", Icon = "i" };
        var location = new Location { Id = Guid.NewGuid(), Name = "L" };
        db.Users.Add(user); db.Types.Add(type); db.Locations.Add(location);
        await db.SaveChangesAsync();
        return (user, type, location);
    }

    private static PeldanyWriteDto NewDto(Guid userId, Guid typeId, Guid locationId, string? desc = "d") => new()
    {
        Description = desc,
        AddDate = DateTime.UtcNow,
        ManufacturingDate = DateTime.UtcNow.AddDays(-1),
        IsHibas = false,
        IsSelejt = false,
        IsSelejtSugg = false,
        SelejtedDate = DateTime.UtcNow,
        SerialNumber = $"SN-{Guid.NewGuid():N}",
        TypeId = typeId,
        ParentId = null,
        FelelosId = userId,
        LocationId = locationId
    };

    [Fact]
    public async Task PostAsync_WithValidDto_ShouldCreatePeldany()
    {
        using var scope = fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var service = GetService(scope);
        var (user, type, location) = await SeedDepsAsync(db);
        var dto = NewDto(user.Id, type.Id, location.Id);

        var created = await service.PostAsync(dto);

        created.Id.Should().NotBeEmpty();
        created.SerialNumber.Should().Be(dto.SerialNumber);
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
    public async Task GetAsync_WhenExists_ShouldReturnPeldany()
    {
        using var scope = fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var service = GetService(scope);
        var (user, type, location) = await SeedDepsAsync(db);
        var created = await service.PostAsync(NewDto(user.Id, type.Id, location.Id));

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
    public async Task PutAsync_WhenExists_ShouldUpdatePeldany()
    {
        using var scope = fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var service = GetService(scope);
        var (user, type, location) = await SeedDepsAsync(db);
        var created = await service.PostAsync(NewDto(user.Id, type.Id, location.Id, "a"));
        var dto = NewDto(user.Id, type.Id, location.Id, "b");

        await service.PutAsync(created.Id, dto);

        var updated = await service.GetAsync(created.Id);
        updated.Description.Should().Be("b");
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
        var (user, type, location) = await SeedDepsAsync(db);
        Func<Task> act = async () => await service.PutAsync(Guid.NewGuid(), NewDto(user.Id, type.Id, location.Id));
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenExists_ShouldDeletePeldany()
    {
        using var scope = fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var service = GetService(scope);
        var (user, type, location) = await SeedDepsAsync(db);
        var created = await service.PostAsync(NewDto(user.Id, type.Id, location.Id));

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
