using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.IntegrationTests.Infrastructure;
using VizsgaremekBackend.Models;
using VizsgaremekBackend.Services;

namespace VizsgaremekBackend.IntegrationTests.Features.DbAccess;

public class TypeServiceTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    private static TypeService GetService(IServiceScope scope) => scope.ServiceProvider.GetRequiredService<TypeService>();

    [Fact]
    public async Task PostAsync_WithValidDto_ShouldCreateType()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);

        var created = await service.PostAsync(new TypeWriteDto { Name = "N", Description = "D", Icon = "I" });

        created.Id.Should().NotBeEmpty();
        created.Name.Should().Be("N");
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
    public async Task GetAsync_WhenExists_ShouldReturnType()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);
        var created = await service.PostAsync(new TypeWriteDto { Name = "N1", Description = "D", Icon = "I" });
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
    public async Task SearchAsync_ShouldReturnOnlyMatchingResults()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);
        var match = await service.PostAsync(new TypeWriteDto { Name = "AlphaTypeX", Description = "D", Icon = "I" });
        var other = await service.PostAsync(new TypeWriteDto { Name = "BetaTypeX", Description = "D", Icon = "I" });

        var result = await service.SearchAsync("AlphaTypeX", 10, 0);

        result.items.Should().Contain(x => x.Id == match.Id);
        result.items.Should().NotContain(x => x.Id == other.Id);
        result.items.Should().OnlyContain(x => x.Name.Contains("AlphaTypeX", StringComparison.OrdinalIgnoreCase));
        result.pageCount.Should().BeGreaterThanOrEqualTo(1);
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
    public async Task PutAsync_WhenExists_ShouldUpdateType()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);
        var created = await service.PostAsync(new TypeWriteDto { Name = "B", Description = "D", Icon = "I" });
        await service.PutAsync(created.Id, new TypeWriteDto { Name = "A", Description = "D2", Icon = "I2" });
        var updated = await service.GetAsync(created.Id);
        updated.Name.Should().Be("A");
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
        Func<Task> act = async () => await service.PutAsync(Guid.NewGuid(), new TypeWriteDto { Name = "A", Description = "D", Icon = "I" });
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenExists_ShouldDeleteType()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);
        var created = await service.PostAsync(new TypeWriteDto { Name = "Del", Description = "D", Icon = "I" });
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
    public async Task GetPeldanys_WhenTypeExists_ShouldReturnOnlyAvailablePeldanys()
    {
        using var scope = fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var service = GetService(scope);
        var type = await service.PostAsync(new TypeWriteDto { Name = "TP", Description = "D", Icon = "I" });
        var user = new User { Id = Guid.NewGuid(), UserName = $"tu_{Guid.NewGuid():N}", Email = $"tu_{Guid.NewGuid():N}@example.com" };
        var location = new Location { Id = Guid.NewGuid(), Name = "L" };
        db.Users.Add(user); db.Locations.Add(location);
        await db.SaveChangesAsync();

        var available = new Peldany { Id = Guid.NewGuid(), TypeId = type.Id, FelelosId = user.Id, LocationId = location.Id, AddDate = DateTime.UtcNow, SerialNumber = $"S1-{Guid.NewGuid():N}", IsHibas = false, IsSelejt = false, IsSelejtSugg = false };
        var taken = new Peldany { Id = Guid.NewGuid(), TypeId = type.Id, FelelosId = user.Id, LocationId = location.Id, AddDate = DateTime.UtcNow, SerialNumber = $"S2-{Guid.NewGuid():N}", IsHibas = false, IsSelejt = false, IsSelejtSugg = false, ActiveKolcsonzesId = Guid.NewGuid() };
        db.Peldanys.AddRange(available, taken);
        await db.SaveChangesAsync();

        var result = await service.GetPeldanys(type.Id, 10, 0);

        result.Should().Contain(x => x.Id == available.Id);
        result.Should().NotContain(x => x.Id == taken.Id);
    }

    [Fact]
    public async Task GetPeldanys_WhenTypeMissing_ShouldThrowNotFoundException()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);
        Func<Task> act = async () => await service.GetPeldanys(Guid.NewGuid(), 10, 0);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetPeldanys_WithNegativeLimit_ShouldThrowBadRequestException()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);
        Func<Task> act = async () => await service.GetPeldanys(Guid.NewGuid(), -1, 0);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task GetPeldanys_WithNegativeOffset_ShouldThrowBadRequestException()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);
        Func<Task> act = async () => await service.GetPeldanys(Guid.NewGuid(), 10, -1);
        await act.Should().ThrowAsync<BadRequestException>();
    }
}
