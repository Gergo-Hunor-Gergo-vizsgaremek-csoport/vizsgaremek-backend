using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.IntegrationTests.Infrastructure;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.IntegrationTests.Features.Api;

public class PeldanyApiTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    private async Task<(Guid userId, Guid typeId, Guid locationId)> SeedDepsAsync()
    {
        using var scope = fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        var user = new User { Id = Guid.NewGuid(), UserName = $"ap_{Guid.NewGuid():N}", Email = $"ap_{Guid.NewGuid():N}@example.com" };
        var type = new VizsgaremekBackend.Models.Type { Id = Guid.NewGuid(), Name = "T", Description = "D", Icon = "i" };
        var location = new Location { Id = Guid.NewGuid(), Name = "L" };
        db.Users.Add(user); db.Types.Add(type); db.Locations.Add(location);
        await db.SaveChangesAsync();
        return (user.Id, type.Id, location.Id);
    }

    private static PeldanyWriteDto NewDto(Guid userId, Guid typeId, Guid locationId, string? desc = "d") => new()
    {
        Description = desc,
        AddDate = DateTime.UtcNow,
        ManufacturingDate = DateTime.UtcNow,
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
    public async Task GetById_WhenExists_ShouldReturnOk()
    {
        var (userId, typeId, locationId) = await SeedDepsAsync();
        var client = fixture.Factory.CreateClient();
        var created = await (await client.PostAsJsonAsync("/Peldany", NewDto(userId, typeId, locationId))).Content.ReadFromJsonAsync<PeldanyReadDto>();

        var resp = await client.GetAsync($"/Peldany/{created!.Id}");

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_WhenMissing_ShouldReturnNotFound()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.GetAsync($"/Peldany/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_WithValidDto_ShouldReturnCreated()
    {
        var (userId, typeId, locationId) = await SeedDepsAsync();
        var client = fixture.Factory.CreateClient();
        var resp = await client.PostAsJsonAsync("/Peldany", NewDto(userId, typeId, locationId));
        resp.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Put_WhenExists_ShouldUpdatePeldany()
    {
        //Arrange
        var (userId, typeId, locationId) = await SeedDepsAsync();
        var client = fixture.Factory.CreateClient();
        var created = await (await client.PostAsJsonAsync("/Peldany", NewDto(userId, typeId, locationId))).Content.ReadFromJsonAsync<PeldanyReadDto>();
        var dto = NewDto(userId, typeId, locationId, "d2");

        //Act
        var putResp = await client.PutAsJsonAsync($"/Peldany/{created!.Id}", dto);

        //Assert
        putResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResp = await client.GetAsync($"/Peldany/{created.Id}");
        var updated = await getResp.Content.ReadFromJsonAsync<PeldanyReadDto>();
        updated!.Description.Should().Be("d2");
    }

    [Fact]
    public async Task Put_WhenMissing_ShouldReturnNotFound()
    {
        var (userId, typeId, locationId) = await SeedDepsAsync();
        var client = fixture.Factory.CreateClient();
        var resp = await client.PutAsJsonAsync($"/Peldany/{Guid.NewGuid()}", NewDto(userId, typeId, locationId));
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WhenExists_ShouldDeletePeldany()
    {
        //Arrange
        var (userId, typeId, locationId) = await SeedDepsAsync();
        var client = fixture.Factory.CreateClient();
        var created = await (await client.PostAsJsonAsync("/Peldany", NewDto(userId, typeId, locationId))).Content.ReadFromJsonAsync<PeldanyReadDto>();

        //Act
        var delResp = await client.DeleteAsync($"/Peldany/{created!.Id}");

        //Assert
        delResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResp = await client.GetAsync($"/Peldany/{created.Id}");
        getResp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WhenMissing_ShouldReturnNotFound()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.DeleteAsync($"/Peldany/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
