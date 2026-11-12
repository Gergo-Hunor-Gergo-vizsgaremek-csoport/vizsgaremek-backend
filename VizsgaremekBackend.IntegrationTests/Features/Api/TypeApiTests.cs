using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.IntegrationTests.Infrastructure;

namespace VizsgaremekBackend.IntegrationTests.Features.Api;

public class TypeApiTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    [Fact]
    public async Task GetById_WhenExists_ShouldReturnOk()
    {
        var client = fixture.Factory.CreateClient();
        var created = await (await client.PostAsJsonAsync("/Type", new TypeWriteDto { Name = "N", Description = "D", Icon = "I" })).Content.ReadFromJsonAsync<TypeReadDto>();
        var resp = await client.GetAsync($"/Type/{created!.Id}");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_WhenMissing_ShouldReturnNotFound()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.GetAsync($"/Type/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Search_WhenMatchExists_ShouldReturnOnlyMatchingResults()
    {
        var client = fixture.Factory.CreateClient();
        var a = await (await client.PostAsJsonAsync("/Type", new TypeWriteDto { Name = "AlphaApiType", Description = "D", Icon = "I" })).Content.ReadFromJsonAsync<TypeReadDto>();
        var b = await (await client.PostAsJsonAsync("/Type", new TypeWriteDto { Name = "BetaApiType", Description = "D", Icon = "I" })).Content.ReadFromJsonAsync<TypeReadDto>();

        var resp = await client.GetAsync("/Type?q=AlphaApiType");
        var array = await resp.Content.ReadFromJsonAsync<TypeReadDto[]>();

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        array!.Should().Contain(x => x.Id == a!.Id);
        array.Should().NotContain(x => x.Id == b!.Id);
    }

    [Fact]
    public async Task Search_WhenLimitTooLarge_ShouldReturnBadRequest()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.GetAsync("/Type?limit=101");
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Search_WhenLimitNegative_ShouldReturnBadRequest()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.GetAsync("/Type?limit=-1");
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Search_WhenOffsetNegative_ShouldReturnBadRequest()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.GetAsync("/Type?offset=-1");
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_WithValidDto_ShouldReturnCreated()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.PostAsJsonAsync("/Type", new TypeWriteDto { Name = "N", Description = "D", Icon = "I" });
        resp.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Put_WhenExists_ShouldUpdateType()
    {
        //Arrange
        var client = fixture.Factory.CreateClient();
        var created = await (await client.PostAsJsonAsync("/Type", new TypeWriteDto { Name = "N", Description = "D", Icon = "I" })).Content.ReadFromJsonAsync<TypeReadDto>();

        //Act
        var putResp = await client.PutAsJsonAsync($"/Type/{created!.Id}", new TypeWriteDto { Name = "N2", Description = "D2", Icon = "I2" });

        //Assert
        putResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResp = await client.GetAsync($"/Type/{created.Id}");
        var updated = await getResp.Content.ReadFromJsonAsync<TypeReadDto>();
        updated!.Name.Should().Be("N2");
        updated.Description.Should().Be("D2");
        updated.Icon.Should().Be("I2");
    }

    [Fact]
    public async Task Put_WhenMissing_ShouldReturnNotFound()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.PutAsJsonAsync($"/Type/{Guid.NewGuid()}", new TypeWriteDto { Name = "N", Description = "D", Icon = "I" });
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WhenExists_ShouldDeleteType()
    {
        //Arrange
        var client = fixture.Factory.CreateClient();
        var created = await (await client.PostAsJsonAsync("/Type", new TypeWriteDto { Name = "N", Description = "D", Icon = "I" })).Content.ReadFromJsonAsync<TypeReadDto>();

        //Act
        var delResp = await client.DeleteAsync($"/Type/{created!.Id}");

        //Assert
        delResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResp = await client.GetAsync($"/Type/{created.Id}");
        getResp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WhenMissing_ShouldReturnNotFound()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.DeleteAsync($"/Type/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPeldanys_WhenMissingType_ShouldReturnNotFound()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.GetAsync($"/Type/{Guid.NewGuid()}/peldanyok");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPeldanys_WhenLimitTooLarge_ShouldReturnBadRequest()
    {
        var client = fixture.Factory.CreateClient();
        var created = await (await client.PostAsJsonAsync("/Type", new TypeWriteDto { Name = "N", Description = "D", Icon = "I" })).Content.ReadFromJsonAsync<TypeReadDto>();
        var resp = await client.GetAsync($"/Type/{created!.Id}/peldanyok?limit=101");
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
