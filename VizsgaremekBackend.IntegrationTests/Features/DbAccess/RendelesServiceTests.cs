using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.IntegrationTests.Infrastructure;
using VizsgaremekBackend.Models;
using VizsgaremekBackend.Services;

namespace VizsgaremekBackend.IntegrationTests.Features.DbAccess;

public class RendelesServiceTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    private static RendelesService GetService(IServiceScope scope) => scope.ServiceProvider.GetRequiredService<RendelesService>();

    private async Task<(User user, VizsgaremekBackend.Models.Type type, Peldany peldany)> SeedAsync(VizsgaremekContext db)
    {
        var user = new User { Id = Guid.NewGuid(), UserName = $"r_{Guid.NewGuid():N}", Email = $"r_{Guid.NewGuid():N}@example.com" };
        var type = new VizsgaremekBackend.Models.Type { Id = Guid.NewGuid(), Name = "T", Description = "D", Icon = "i" };
        var location = new Location { Id = Guid.NewGuid(), Name = "L" };
        db.Users.Add(user); db.Types.Add(type); db.Locations.Add(location);
        await db.SaveChangesAsync();
        var peldany = new Peldany { Id = Guid.NewGuid(), TypeId = type.Id, FelelosId = user.Id, LocationId = location.Id, AddDate = DateTime.UtcNow, SerialNumber = $"SN-{Guid.NewGuid():N}", IsHibas = false, IsSelejt = false, IsSelejtSugg = false };
        db.Peldanys.Add(peldany);
        await db.SaveChangesAsync();
        return (user, type, peldany);
    }

    private static RendelesWriteDto NewDto(Guid userId, Guid typeId, int qty = 1, int completed = 0) => new()
    {
        UserId = userId, TypeId = typeId, Date = DateTime.UtcNow, Quantity = qty, CompletedQuantity = completed
    };

    [Fact]
    public async Task PostAsync_WithValidDto_ShouldCreateRendeles()
    {
        using var scope = fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var service = GetService(scope);
        var (user, type, _) = await SeedAsync(db);

        var created = await service.PostAsync(NewDto(user.Id, type.Id));

        created.Id.Should().NotBeEmpty();
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
    public async Task GetAsync_WhenExists_ShouldReturnRendeles()
    {
        using var scope = fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var service = GetService(scope);
        var (user, type, _) = await SeedAsync(db);
        var created = await service.PostAsync(NewDto(user.Id, type.Id));

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
    public async Task PutAsync_WhenExists_ShouldUpdateRendeles()
    {
        using var scope = fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var service = GetService(scope);
        var (user, type, _) = await SeedAsync(db);
        var created = await service.PostAsync(NewDto(user.Id, type.Id, 1, 0));

        await service.PutAsync(created.Id, NewDto(user.Id, type.Id, 1, 1));

        var updated = await service.GetAsync(created.Id);
        updated.CompletedQuantity.Should().Be(1);
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
        var (user, type, _) = await SeedAsync(db);
        Func<Task> act = async () => await service.PutAsync(Guid.NewGuid(), NewDto(user.Id, type.Id));
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenExists_ShouldDeleteRendeles()
    {
        using var scope = fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var service = GetService(scope);
        var (user, type, _) = await SeedAsync(db);
        var created = await service.PostAsync(NewDto(user.Id, type.Id));

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
    public async Task AssignPeldanysAsync_WithValidData_ShouldCreateKolcsonzesek()
    {
        using var scope = fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var service = GetService(scope);
        var (user, type, peldany) = await SeedAsync(db);
        var rendeles = await service.PostAsync(NewDto(user.Id, type.Id));

        var result = await service.AssignPeldanysAsync(rendeles.Id, new[] { new KolcsonzesWriteDto { UserId = user.Id, PeldanyId = peldany.Id, Date = DateTime.UtcNow } });

        result.Should().HaveCount(1);
        result[0].PeldanyId.Should().Be(peldany.Id);

        // modification is actually persisted
        var p = db.Peldanys.First(x => x.Id == peldany.Id);
        p.ActiveKolcsonzesId.Should().Be(result[0].Id);
        db.Kolcsonzess.Any(x => x.Id == result[0].Id).Should().BeTrue();
    }

    [Fact]
    public async Task AssignPeldanysAsync_WithNullArray_ShouldThrowArgumentNullException()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);
        Func<Task> act = async () => await service.AssignPeldanysAsync(Guid.NewGuid(), null!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AssignPeldanysAsync_WithEmptyArray_ShouldThrowBadRequestException()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);
        Func<Task> act = async () => await service.AssignPeldanysAsync(Guid.NewGuid(), Array.Empty<KolcsonzesWriteDto>());
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task AssignPeldanysAsync_WhenRendelesMissing_ShouldThrowNotFoundException()
    {
        using var scope = fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var service = GetService(scope);
        var (user, _, peldany) = await SeedAsync(db);
        Func<Task> act = async () => await service.AssignPeldanysAsync(Guid.NewGuid(), new[] { new KolcsonzesWriteDto { UserId = user.Id, PeldanyId = peldany.Id, Date = DateTime.UtcNow } });
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task AssignPeldanysAsync_WhenPeldanyMissing_ShouldThrowNotFoundException()
    {
        using var scope = fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var service = GetService(scope);
        var (user, type, _) = await SeedAsync(db);
        var rendeles = await service.PostAsync(NewDto(user.Id, type.Id));
        Func<Task> act = async () => await service.AssignPeldanysAsync(rendeles.Id, new[] { new KolcsonzesWriteDto { UserId = user.Id, PeldanyId = Guid.NewGuid(), Date = DateTime.UtcNow } });
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task AssignPeldanysAsync_WhenPeldanyAlreadyAssigned_ShouldThrowConflictException()
    {
        using var scope = fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var service = GetService(scope);
        var (user, type, peldany) = await SeedAsync(db);
        var rendeles = await service.PostAsync(NewDto(user.Id, type.Id));
        var dto = new KolcsonzesWriteDto { UserId = user.Id, PeldanyId = peldany.Id, Date = DateTime.UtcNow };
        await service.AssignPeldanysAsync(rendeles.Id, new[] { dto });

        Func<Task> act = async () => await service.AssignPeldanysAsync(rendeles.Id, new[] { dto });

        await act.Should().ThrowAsync<ConflictException>();
    }
}
