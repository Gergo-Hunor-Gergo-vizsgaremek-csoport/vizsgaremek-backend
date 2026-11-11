using System.Net;
using FluentAssertions;
using VizsgaremekBackend.IntegrationTests.Infrastructure;

namespace VizsgaremekBackend.IntegrationTests.Features.Api;

public class AdminApiTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    [Fact]
    public async Task MigrateDatabase_WhenCalled_ShouldReturnOk()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.GetAsync("/Admin/MigrateDatabase");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
