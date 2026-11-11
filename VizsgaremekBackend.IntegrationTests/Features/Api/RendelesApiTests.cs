using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.IntegrationTests.Infrastructure;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.IntegrationTests.Features.Api;

public class RendelesApiTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    private async Task<(Guid userId, Guid typeId, Guid peldanyId)> SeedAsync()
    {
        using var scope = fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var user = new User { Id = Guid.NewGuid(), UserName = $"ar_{Guid.NewGuid():N}", Email = $"ar_{Guid.NewGuid():N}@example.com" };
        var type = new VizsgaremekBackend.Models.Type { Id = Guid.NewGuid(), Name = "T", Description = "D", Icon = "i" };
        var location = new Location { Id = Guid.NewGuid(), Name = "L" };
        db.Users.Add(user); db.Types.Add(type); db.Locations.Add(location);
        await db.SaveChangesAsync();
        var peldany = new Peldany { Id = Guid.NewGuid(), TypeId = type.Id, FelelosId = user.Id, LocationId = location.Id, AddDate = DateTime.UtcNow, SerialNumber = $"SN-{Guid.NewGuid():N}", IsHibas = false, IsSelejt = false, IsSelejtSugg = false };
        db.Peldanys.Add(peldany);
        await db.SaveChangesAsync();
        return (user.Id, type.Id, peldany.Id);
    }

    [Fact]
    public async Task GetById_WhenExists_ShouldReturnOk()
    {
        var (userId, typeId, _) = await SeedAsync();
        var client = fixture.Factory.CreateClient();
        var created = await (await client.PostAsJsonAsync("/Rendeles", new RendelesWriteDto { UserId = userId, TypeId = typeId, Date = DateTime.UtcNow, Quantity = 1, CompletedQuantity = 0 })).Content.ReadFromJsonAsync<RendelesReadDto>();

        var resp = await client.GetAsync($"/Rendeles/{created!.Id}");

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_WhenMissing_ShouldReturnNotFound()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.GetAsync($"/Rendeles/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_WithValidDto_ShouldReturnCreated()
    {
        var (userId, typeId, _) = await SeedAsync();
        var client = fixture.Factory.CreateClient();
        var resp = await client.PostAsJsonAsync("/Rendeles", new RendelesWriteDto { UserId = userId, TypeId = typeId, Date = DateTime.UtcNow, Quantity = 1, CompletedQuantity = 0 });
        resp.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Put_WhenExists_ShouldUpdateRendeles()
    {
        //Arrange
        var (userId, typeId, _) = await SeedAsync();
        var client = fixture.Factory.CreateClient();
        var created = await (await client.PostAsJsonAsync("/Rendeles", new RendelesWriteDto { UserId = userId, TypeId = typeId, Date = DateTime.UtcNow, Quantity = 1, CompletedQuantity = 0 })).Content.ReadFromJsonAsync<RendelesReadDto>();

        //Act
        var putResp = await client.PutAsJsonAsync($"/Rendeles/{created!.Id}", new RendelesWriteDto { UserId = userId, TypeId = typeId, Date = DateTime.UtcNow, Quantity = 2, CompletedQuantity = 1 });

        //Assert
        putResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResp = await client.GetAsync($"/Rendeles/{created.Id}");
        var updated = await getResp.Content.ReadFromJsonAsync<RendelesReadDto>();
        updated!.Quantity.Should().Be(2);
        updated.CompletedQuantity.Should().Be(1);
    }

    [Fact]
    public async Task Put_WhenMissing_ShouldReturnNotFound()
    {
        var (userId, typeId, _) = await SeedAsync();
        var client = fixture.Factory.CreateClient();
        var resp = await client.PutAsJsonAsync($"/Rendeles/{Guid.NewGuid()}", new RendelesWriteDto { UserId = userId, TypeId = typeId, Date = DateTime.UtcNow, Quantity = 1, CompletedQuantity = 0 });
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WhenExists_ShouldDeleteRendeles()
    {
        //Arrange
        var (userId, typeId, _) = await SeedAsync();
        var client = fixture.Factory.CreateClient();
        var created = await (await client.PostAsJsonAsync("/Rendeles", new RendelesWriteDto { UserId = userId, TypeId = typeId, Date = DateTime.UtcNow, Quantity = 1, CompletedQuantity = 0 })).Content.ReadFromJsonAsync<RendelesReadDto>();

        //Act
        var delResp = await client.DeleteAsync($"/Rendeles/{created!.Id}");

        //Assert
        delResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResp = await client.GetAsync($"/Rendeles/{created.Id}");
        getResp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WhenMissing_ShouldReturnNotFound()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.DeleteAsync($"/Rendeles/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Assign_WhenValid_ShouldCreateKolcsonzesek()
    {
        //Arrange
        var (userId, typeId, peldanyId) = await SeedAsync();
        var client = fixture.Factory.CreateClient();
        var created = await (await client.PostAsJsonAsync("/Rendeles", new RendelesWriteDto { UserId = userId, TypeId = typeId, Date = DateTime.UtcNow, Quantity = 1, CompletedQuantity = 0 })).Content.ReadFromJsonAsync<RendelesReadDto>();

        //Act
        var assignResp = await client.PostAsJsonAsync($"/Rendeles/{created!.Id}/assign", new[] { new KolcsonzesWriteDto { UserId = userId, PeldanyId = peldanyId, Date = DateTime.UtcNow } });

        //Assert
        assignResp.StatusCode.Should().Be(HttpStatusCode.Created);

        using (var scope = fixture.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
            var p = db.Peldanys.First(x => x.Id == peldanyId);
            p.ActiveKolcsonzesId.Should().NotBeNull();
            db.Kolcsonzess.Any(x => x.Id == p.ActiveKolcsonzesId).Should().BeTrue();
        }
    }

    [Fact]
    public async Task Assign_WhenRendelesMissing_ShouldReturnNotFound()
    {
        var (userId, _, peldanyId) = await SeedAsync();
        var client = fixture.Factory.CreateClient();

        var resp = await client.PostAsJsonAsync($"/Rendeles/{Guid.NewGuid()}/assign", new[] { new KolcsonzesWriteDto { UserId = userId, PeldanyId = peldanyId, Date = DateTime.UtcNow } });

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Assign_WhenPeldanyMissing_ShouldReturnNotFound()
    {
        var (userId, typeId, _) = await SeedAsync();
        var client = fixture.Factory.CreateClient();
        var created = await (await client.PostAsJsonAsync("/Rendeles", new RendelesWriteDto { UserId = userId, TypeId = typeId, Date = DateTime.UtcNow, Quantity = 1, CompletedQuantity = 0 })).Content.ReadFromJsonAsync<RendelesReadDto>();

        var resp = await client.PostAsJsonAsync($"/Rendeles/{created!.Id}/assign", new[] { new KolcsonzesWriteDto { UserId = userId, PeldanyId = Guid.NewGuid(), Date = DateTime.UtcNow } });

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
