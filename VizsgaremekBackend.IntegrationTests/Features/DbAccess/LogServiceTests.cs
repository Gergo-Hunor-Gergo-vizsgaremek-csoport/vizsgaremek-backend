using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.IntegrationTests.Infrastructure;
using VizsgaremekBackend.Models;
using VizsgaremekBackend.Services;

namespace VizsgaremekBackend.IntegrationTests.Features.DbAccess;

public class LogServiceTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    private static LogService GetService(IServiceScope scope) => scope.ServiceProvider.GetRequiredService<LogService>();

    private async Task<User> CreateUserAsync(VizsgaremekContext db)
    {
        var user = new User { Id = Guid.NewGuid(), UserName = $"log_{Guid.NewGuid():N}", Email = $"log_{Guid.NewGuid():N}@example.com" };
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return user;
    }

    [Fact]
    public async Task PostAsync_WithValidDto_ShouldCreateLog()
    {
        using var scope = fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var service = GetService(scope);
        var user = await CreateUserAsync(db);

        var created = await service.PostAsync(new LogWriteDto { Type = "Info", Message = "m", Date = DateTime.UtcNow, UserId = user.Id });

        created.Id.Should().NotBeEmpty();
        created.Type.Should().Be("Info");
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
    public async Task GetAsync_WhenExists_ShouldReturnLog()
    {
        using var scope = fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var service = GetService(scope);
        var user = await CreateUserAsync(db);
        var created = await service.PostAsync(new LogWriteDto { Type = "T", Message = "m", Date = DateTime.UtcNow, UserId = user.Id });

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
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var service = GetService(scope);
        var user = await CreateUserAsync(db);
        var match = await service.PostAsync(new LogWriteDto { Type = "AlphaType", Message = "m1", Date = DateTime.UtcNow, UserId = user.Id });
        var other = await service.PostAsync(new LogWriteDto { Type = "BetaType", Message = "m2", Date = DateTime.UtcNow, UserId = user.Id });

        var result = await service.SearchAsync("AlphaType", 10, 0);

        result.Should().Contain(x => x.Id == match.Id);
        result.Should().NotContain(x => x.Id == other.Id);
        result.Should().OnlyContain(x => x.Type.Contains("AlphaType", StringComparison.OrdinalIgnoreCase));
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
    public async Task PutAsync_WhenExists_ShouldUpdateLog()
    {
        using var scope = fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var service = GetService(scope);
        var user = await CreateUserAsync(db);
        var created = await service.PostAsync(new LogWriteDto { Type = "A", Message = "m", Date = DateTime.UtcNow, UserId = user.Id });

        await service.PutAsync(created.Id, new LogWriteDto { Type = "B", Message = "m2", Date = created.Date, UserId = user.Id });

        var updated = await service.GetAsync(created.Id);
        updated.Type.Should().Be("B");
        updated.Message.Should().Be("m2");
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
        var user = await CreateUserAsync(db);

        Func<Task> act = async () => await service.PutAsync(Guid.NewGuid(), new LogWriteDto { Type = "A", Message = "m", Date = DateTime.UtcNow, UserId = user.Id });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenExists_ShouldDeleteLog()
    {
        using var scope = fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var service = GetService(scope);
        var user = await CreateUserAsync(db);
        var created = await service.PostAsync(new LogWriteDto { Type = "A", Message = "m", Date = DateTime.UtcNow, UserId = user.Id });

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
