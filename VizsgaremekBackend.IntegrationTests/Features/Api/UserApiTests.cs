using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.IntegrationTests.Infrastructure;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.IntegrationTests.Features.Api;

public class UserApiTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    private static UserWriteDto NewWrite(string? prefix = null)
    {
        var uname = $"{prefix ?? "apiuser"}_{Guid.NewGuid():N}"[..24];
        return new UserWriteDto { UserName = uname, Email = $"{uname}@example.com", Password = "Password1" };
    }

    [Fact]
    public async Task Search_WhenMatchExists_ShouldReturnOnlyMatchingResults()
    {
        var client = fixture.Factory.CreateClient();
        var w1 = NewWrite("AlphaUser");
        var w2 = NewWrite("BetaUser");
        var a = await (await client.PostAsJsonAsync("/User", w1)).Content.ReadFromJsonAsync<UserReadDto>();
        var b = await (await client.PostAsJsonAsync("/User", w2)).Content.ReadFromJsonAsync<UserReadDto>();

        var resp = await client.GetAsync($"/User?q={w1.UserName}");
        var list = await resp.Content.ReadFromJsonAsync<List<UserReadDto>>();

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        list.Should().Contain(x => x.Id == a!.Id);
        list.Should().NotContain(x => x.Id == b!.Id);
    }

    [Fact]
    public async Task Search_WhenLimitTooLarge_ShouldReturnBadRequest()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.GetAsync("/User?limit=101");
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Search_WhenLimitNegative_ShouldReturnBadRequest()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.GetAsync("/User?limit=-1");
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Search_WhenOffsetNegative_ShouldReturnBadRequest()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.GetAsync("/User?offset=-1");
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetById_WhenExists_ShouldReturnOk()
    {
        var client = fixture.Factory.CreateClient();
        var created = await (await client.PostAsJsonAsync("/User", NewWrite())).Content.ReadFromJsonAsync<UserReadDto>();
        var resp = await client.GetAsync($"/User/{created!.Id}");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_WhenMissing_ShouldReturnNotFound()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.GetAsync($"/User/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_WithValidDto_ShouldReturnCreated()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.PostAsJsonAsync("/User", NewWrite());
        resp.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Post_WithMissingPassword_ShouldReturnBadRequest()
    {
        var client = fixture.Factory.CreateClient();
        var w = NewWrite();
        w.Password = null;
        var resp = await client.PostAsJsonAsync("/User", w);
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_WhenExists_ShouldUpdateUser()
    {
        //Arrange
        var client = fixture.Factory.CreateClient();
        var w = NewWrite();
        var created = await (await client.PostAsJsonAsync("/User", w)).Content.ReadFromJsonAsync<UserReadDto>();
        w.Password = null;
        var newEmail = $"upd_{Guid.NewGuid():N}@example.com";
        w.Email = newEmail;

        //Act
        var putResp = await client.PutAsJsonAsync($"/User/{created!.Id}", w);

        //Assert
        putResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResp = await client.GetAsync($"/User/{created.Id}");
        var updated = await getResp.Content.ReadFromJsonAsync<UserReadDto>();
        updated!.Email.Should().Be(newEmail);
    }

    [Fact]
    public async Task Put_WhenMissing_ShouldReturnNotFound()
    {
        var client = fixture.Factory.CreateClient();
        var w = NewWrite();
        w.Password = null;
        var resp = await client.PutAsJsonAsync($"/User/{Guid.NewGuid()}", w);
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WhenExists_ShouldDeleteUser()
    {
        //Arrange
        var client = fixture.Factory.CreateClient();
        var created = await (await client.PostAsJsonAsync("/User", NewWrite())).Content.ReadFromJsonAsync<UserReadDto>();

        //Act
        var delResp = await client.DeleteAsync($"/User/{created!.Id}");

        //Assert
        delResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResp = await client.GetAsync($"/User/{created.Id}");
        getResp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WhenMissing_ShouldReturnNotFound()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.DeleteAsync($"/User/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetKolcsonzesek_WhenMissing_ShouldReturnNotFound()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.GetAsync($"/User/{Guid.NewGuid()}/kolcsonzesek");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetRendelesek_WhenMissing_ShouldReturnNotFound()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.GetAsync($"/User/{Guid.NewGuid()}/rendelesek");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetFelelossegek_WhenMissing_ShouldReturnNotFound()
    {
        var client = fixture.Factory.CreateClient();
        var resp = await client.GetAsync($"/User/{Guid.NewGuid()}/felelossegek");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SetSysAdminRole_WhenSetTrue_ShouldAssignRole()
    {
        //Arrange
        var client = fixture.Factory.CreateClient();
        var created = await (await client.PostAsJsonAsync("/User", NewWrite())).Content.ReadFromJsonAsync<UserReadDto>();

        //Act
        var resp = await client.PutAsync($"/User/{created!.Id}/SysAdmin?set=true", null);

        //Assert
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        using (var scope = fixture.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var user = await userManager.FindByIdAsync(created.Id.ToString());
            (await userManager.IsInRoleAsync(user!, "SysAdmin")).Should().BeTrue();
        }
    }

    [Fact]
    public async Task SetUserAdminRole_WhenSetTrue_ShouldAssignRole()
    {
        //Arrange
        var client = fixture.Factory.CreateClient();
        var created = await (await client.PostAsJsonAsync("/User", NewWrite())).Content.ReadFromJsonAsync<UserReadDto>();

        //Act
        var resp = await client.PutAsync($"/User/{created!.Id}/UserAdmin?set=true", null);

        //Assert
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        using (var scope = fixture.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var user = await userManager.FindByIdAsync(created.Id.ToString());
            (await userManager.IsInRoleAsync(user!, "UserAdmin")).Should().BeTrue();
        }
    }

    [Fact]
    public async Task SetDeviceAdminRole_WhenSetTrue_ShouldAssignRole()
    {
        //Arrange
        var client = fixture.Factory.CreateClient();
        var created = await (await client.PostAsJsonAsync("/User", NewWrite())).Content.ReadFromJsonAsync<UserReadDto>();

        //Act
        var resp = await client.PutAsync($"/User/{created!.Id}/DeviceAdmin?set=true", null);

        //Assert
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        using (var scope = fixture.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var user = await userManager.FindByIdAsync(created.Id.ToString());
            (await userManager.IsInRoleAsync(user!, "DeviceAdmin")).Should().BeTrue();
        }
    }
}
