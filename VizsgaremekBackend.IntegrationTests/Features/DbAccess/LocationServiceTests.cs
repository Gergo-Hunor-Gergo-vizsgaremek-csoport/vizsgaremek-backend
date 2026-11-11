using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.IntegrationTests.Infrastructure;
using VizsgaremekBackend.Services;

namespace VizsgaremekBackend.IntegrationTests.Features.DbAccess;

public class LocationServiceTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    private static LocationService GetService(IServiceScope scope) => scope.ServiceProvider.GetRequiredService<LocationService>();

    [Fact]
    public async Task PostAsync_WithValidDto_ShouldCreateLocation()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);

        var created = await service.PostAsync(new LocationWriteDto { Name = "CreateLoc" });

        created.Id.Should().NotBeEmpty();
        created.Name.Should().Be("CreateLoc");
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
    public async Task GetAsync_WhenExists_ShouldReturnLocation()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);
        var created = await service.PostAsync(new LocationWriteDto { Name = "GetLoc" });

        var fetched = await service.GetAsync(created.Id);

        fetched.Should().BeEquivalentTo(created);
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
        var match = await service.PostAsync(new LocationWriteDto { Name = "AlphaSearchLoc" });
        var other = await service.PostAsync(new LocationWriteDto { Name = "BetaSearchLoc" });

        var result = await service.SearchAsync("AlphaSearch", 10, 0);

        result.Should().Contain(x => x.Id == match.Id);
        result.Should().NotContain(x => x.Id == other.Id);
        result.Should().OnlyContain(x => x.Name.Contains("AlphaSearch", StringComparison.OrdinalIgnoreCase));
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
    public async Task PutAsync_WhenExists_ShouldUpdateLocation()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);
        var created = await service.PostAsync(new LocationWriteDto { Name = "Before" });

        await service.PutAsync(created.Id, new LocationWriteDto { Name = "After" });

        var updated = await service.GetAsync(created.Id);
        updated.Name.Should().Be("After");
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

        Func<Task> act = async () => await service.PutAsync(Guid.NewGuid(), new LocationWriteDto { Name = "x" });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenExists_ShouldDeleteLocation()
    {
        using var scope = fixture.CreateScope();
        var service = GetService(scope);
        var created = await service.PostAsync(new LocationWriteDto { Name = "DeleteMe" });

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
