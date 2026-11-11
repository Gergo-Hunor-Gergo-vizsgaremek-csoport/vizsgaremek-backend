using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.IntegrationTests.Infrastructure;

namespace VizsgaremekBackend.IntegrationTests.Features.Api;

public class LocationApiTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    [Fact]
    public async Task GetById_WhenExists_ShouldReturnOk()
    {
        var client = fixture.Factory.CreateClient();
        var created = await (await client.PostAsJsonAsync("/Location", new LocationWriteDto { Name = "GetLoc" })).Content.ReadFromJsonAsync<LocationReadDto>();

        var resp = await client.GetAsync($"/Location/{created!.Id}");

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_WhenMissing_ShouldReturnNotFound()
    {
        var client = fixture.Factory.CreateClient();

        var resp = await client.GetAsync($"/Location/{Guid.NewGuid()}");

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Search_WhenMatchExists_ShouldReturnOnlyMatchingResults()
    {
        var client = fixture.Factory.CreateClient();
        var a = await (await client.PostAsJsonAsync("/Location", new LocationWriteDto { Name = "AlphaApiLoc" })).Content.ReadFromJsonAsync<LocationReadDto>();
        var b = await (await client.PostAsJsonAsync("/Location", new LocationWriteDto { Name = "BetaApiLoc" })).Content.ReadFromJsonAsync<LocationReadDto>();

        var resp = await client.GetAsync("/Location?q=AlphaApiLoc");
        var list = await resp.Content.ReadFromJsonAsync<List<LocationReadDto>>();

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        list.Should().Contain(x => x.Id == a!.Id);
        list.Should().NotContain(x => x.Id == b!.Id);
    }

    [Fact]
    public async Task Search_WhenLimitTooLarge_ShouldReturnBadRequest()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.GetAsync("/Location?limit=101");
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Search_WhenLimitNegative_ShouldReturnBadRequest()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.GetAsync("/Location?limit=-1");
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Search_WhenOffsetNegative_ShouldReturnBadRequest()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.GetAsync("/Location?offset=-1");
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_WithValidDto_ShouldReturnCreated()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.PostAsJsonAsync("/Location", new LocationWriteDto { Name = "PostLoc" });
        resp.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Put_WhenExists_ShouldUpdateLocation()
    {
        //Arrange
        var client = fixture.Factory.CreateClient();
        var created = await (await client.PostAsJsonAsync("/Location", new LocationWriteDto { Name = "Old" })).Content.ReadFromJsonAsync<LocationReadDto>();

        //Act
        var putResp = await client.PutAsJsonAsync($"/Location/{created!.Id}", new LocationWriteDto { Name = "New" });

        //Assert
        putResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResp = await client.GetAsync($"/Location/{created.Id}");
        var updated = await getResp.Content.ReadFromJsonAsync<LocationReadDto>();
        updated!.Name.Should().Be("New");
    }

    [Fact]
    public async Task Put_WhenMissing_ShouldReturnNotFound()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.PutAsJsonAsync($"/Location/{Guid.NewGuid()}", new LocationWriteDto { Name = "New" });
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WhenExists_ShouldDeleteLocation()
    {
        //Arrange
        var client = fixture.Factory.CreateClient();
        var created = await (await client.PostAsJsonAsync("/Location", new LocationWriteDto { Name = "Del" })).Content.ReadFromJsonAsync<LocationReadDto>();

        //Act
        var delResp = await client.DeleteAsync($"/Location/{created!.Id}");

        //Assert
        delResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResp = await client.GetAsync($"/Location/{created.Id}");
        getResp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WhenMissing_ShouldReturnNotFound()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.DeleteAsync($"/Location/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
