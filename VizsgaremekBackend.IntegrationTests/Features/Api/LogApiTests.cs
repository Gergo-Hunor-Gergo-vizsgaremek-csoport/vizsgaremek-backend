using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.IntegrationTests.Infrastructure;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.IntegrationTests.Features.Api;

public class LogApiTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    private async Task<Guid> CreateUserIdAsync()
    {
        using var scope = fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var user = new User { Id = Guid.NewGuid(), UserName = $"alog_{Guid.NewGuid():N}", Email = $"alog_{Guid.NewGuid():N}@example.com" };
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return user.Id;
    }

    [Fact]
    public async Task GetById_WhenExists_ShouldReturnOk()
    {
        var userId = await CreateUserIdAsync();
        var client = fixture.Factory.CreateClient();
        var created = await (await client.PostAsJsonAsync("/Log", new LogWriteDto { Type = "T", Message = "m", Date = DateTime.UtcNow, UserId = userId })).Content.ReadFromJsonAsync<LogReadDto>();

        var resp = await client.GetAsync($"/Log/{created!.Id}");

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_WhenMissing_ShouldReturnNotFound()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.GetAsync($"/Log/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Search_WhenMatchExists_ShouldReturnOnlyMatchingResults()
    {
        var userId = await CreateUserIdAsync();
        var client = fixture.Factory.CreateClient();
        var a = await (await client.PostAsJsonAsync("/Log", new LogWriteDto { Type = "AlphaApiLog", Message = "m", Date = DateTime.UtcNow, UserId = userId })).Content.ReadFromJsonAsync<LogReadDto>();
        var b = await (await client.PostAsJsonAsync("/Log", new LogWriteDto { Type = "BetaApiLog", Message = "m", Date = DateTime.UtcNow, UserId = userId })).Content.ReadFromJsonAsync<LogReadDto>();

        var resp = await client.GetAsync("/Log?q=AlphaApiLog");
        var list = await resp.Content.ReadFromJsonAsync<List<LogReadDto>>();

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        list.Should().Contain(x => x.Id == a!.Id);
        list.Should().NotContain(x => x.Id == b!.Id);
    }

    [Fact]
    public async Task Search_WhenLimitTooLarge_ShouldReturnBadRequest()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.GetAsync("/Log?limit=101");
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Search_WhenLimitNegative_ShouldReturnBadRequest()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.GetAsync("/Log?limit=-1");
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Search_WhenOffsetNegative_ShouldReturnBadRequest()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.GetAsync("/Log?offset=-1");
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_WithValidDto_ShouldReturnCreated()
    {
        var userId = await CreateUserIdAsync();
        var client = fixture.Factory.CreateClient();
        var resp = await client.PostAsJsonAsync("/Log", new LogWriteDto { Type = "T", Message = "m", Date = DateTime.UtcNow, UserId = userId });
        resp.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Put_WhenExists_ShouldUpdateLog()
    {
        //Arrange
        var userId = await CreateUserIdAsync();
        var client = fixture.Factory.CreateClient();
        var created = await (await client.PostAsJsonAsync("/Log", new LogWriteDto { Type = "T", Message = "m", Date = DateTime.UtcNow, UserId = userId })).Content.ReadFromJsonAsync<LogReadDto>();

        //Act
        var putResp = await client.PutAsJsonAsync($"/Log/{created!.Id}", new LogWriteDto { Type = "T2", Message = "m2", Date = DateTime.UtcNow, UserId = userId });

        //Assert
        putResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResp = await client.GetAsync($"/Log/{created.Id}");
        var updated = await getResp.Content.ReadFromJsonAsync<LogReadDto>();
        updated!.Type.Should().Be("T2");
        updated.Message.Should().Be("m2");
    }

    [Fact]
    public async Task Put_WhenMissing_ShouldReturnNotFound()
    {
        var userId = await CreateUserIdAsync();
        var client = fixture.Factory.CreateClient();
        var resp = await client.PutAsJsonAsync($"/Log/{Guid.NewGuid()}", new LogWriteDto { Type = "T", Message = "m", Date = DateTime.UtcNow, UserId = userId });
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WhenExists_ShouldDeleteLog()
    {
        //Arrange
        var userId = await CreateUserIdAsync();
        var client = fixture.Factory.CreateClient();
        var created = await (await client.PostAsJsonAsync("/Log", new LogWriteDto { Type = "T", Message = "m", Date = DateTime.UtcNow, UserId = userId })).Content.ReadFromJsonAsync<LogReadDto>();

        //Act
        var delResp = await client.DeleteAsync($"/Log/{created!.Id}");

        //Assert
        delResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResp = await client.GetAsync($"/Log/{created.Id}");
        getResp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WhenMissing_ShouldReturnNotFound()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.DeleteAsync($"/Log/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
