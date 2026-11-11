using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.IntegrationTests.Infrastructure;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.IntegrationTests.Features.Api;

public class KolcsonzesApiTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    private async Task<(Guid userId, Guid peldanyId)> SeedAsync()
    {
        using var scope = fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var user = new User { Id = Guid.NewGuid(), UserName = $"ak_{Guid.NewGuid():N}", Email = $"ak_{Guid.NewGuid():N}@example.com" };
        var type = new VizsgaremekBackend.Models.Type { Id = Guid.NewGuid(), Name = "T", Description = "D", Icon = "i" };
        var location = new Location { Id = Guid.NewGuid(), Name = "L" };
        db.Users.Add(user); db.Types.Add(type); db.Locations.Add(location);
        await db.SaveChangesAsync();
        var peldany = new Peldany { Id = Guid.NewGuid(), TypeId = type.Id, FelelosId = user.Id, LocationId = location.Id, AddDate = DateTime.UtcNow, SerialNumber = $"SN-{Guid.NewGuid():N}", IsHibas = false, IsSelejt = false, IsSelejtSugg = false };
        db.Peldanys.Add(peldany);
        await db.SaveChangesAsync();
        return (user.Id, peldany.Id);
    }

    [Fact]
    public async Task GetById_WhenExists_ShouldReturnOk()
    {
        var (userId, peldanyId) = await SeedAsync();
        var client = fixture.Factory.CreateClient();
        var created = await (await client.PostAsJsonAsync("/Kolcsonzes", new KolcsonzesWriteDto { UserId = userId, PeldanyId = peldanyId, Date = DateTime.UtcNow })).Content.ReadFromJsonAsync<KolcsonzesReadDto>();

        var resp = await client.GetAsync($"/Kolcsonzes/{created!.Id}");

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_WhenMissing_ShouldReturnNotFound()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.GetAsync($"/Kolcsonzes/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_WithValidDto_ShouldReturnCreated()
    {
        var (userId, peldanyId) = await SeedAsync();
        var client = fixture.Factory.CreateClient();
        var resp = await client.PostAsJsonAsync("/Kolcsonzes", new KolcsonzesWriteDto { UserId = userId, PeldanyId = peldanyId, Date = DateTime.UtcNow });
        resp.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Put_WhenExists_ShouldUpdateKolcsonzes()
    {
        //Arrange
        var (userId, peldanyId) = await SeedAsync();
        var client = fixture.Factory.CreateClient();
        var created = await (await client.PostAsJsonAsync("/Kolcsonzes", new KolcsonzesWriteDto { UserId = userId, PeldanyId = peldanyId, Date = DateTime.UtcNow })).Content.ReadFromJsonAsync<KolcsonzesReadDto>();
        var newExpiration = DateTime.UtcNow.AddDays(5);

        //Act
        var putResp = await client.PutAsJsonAsync($"/Kolcsonzes/{created!.Id}", new KolcsonzesWriteDto { UserId = userId, PeldanyId = peldanyId, Date = DateTime.UtcNow, ExpirationDate = newExpiration });

        //Assert
        putResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResp = await client.GetAsync($"/Kolcsonzes/{created.Id}");
        var updated = await getResp.Content.ReadFromJsonAsync<KolcsonzesReadDto>();
        updated!.ExpirationDate.Should().NotBeNull();
        (updated.ExpirationDate!.Value - newExpiration).Duration().Should().BeLessThanOrEqualTo(TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task Put_WhenMissing_ShouldReturnNotFound()
    {
        var (userId, peldanyId) = await SeedAsync();
        var client = fixture.Factory.CreateClient();
        var resp = await client.PutAsJsonAsync($"/Kolcsonzes/{Guid.NewGuid()}", new KolcsonzesWriteDto { UserId = userId, PeldanyId = peldanyId, Date = DateTime.UtcNow });
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WhenExists_ShouldDeleteKolcsonzes()
    {
        //Arrange
        var (userId, peldanyId) = await SeedAsync();
        var client = fixture.Factory.CreateClient();
        var created = await (await client.PostAsJsonAsync("/Kolcsonzes", new KolcsonzesWriteDto { UserId = userId, PeldanyId = peldanyId, Date = DateTime.UtcNow })).Content.ReadFromJsonAsync<KolcsonzesReadDto>();

        //Act
        var delResp = await client.DeleteAsync($"/Kolcsonzes/{created!.Id}");

        //Assert
        delResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResp = await client.GetAsync($"/Kolcsonzes/{created.Id}");
        getResp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WhenMissing_ShouldReturnNotFound()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.DeleteAsync($"/Kolcsonzes/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
