using System.Security.Claims;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VizsgaremekBackend.Controllers;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.Interfaces;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.UnitTests.Features;

public class UserControllerTests
{
    private static UserController CreateController(
        Mock<IUserService>? userServiceMock = null,
        Mock<IUserRoleService>? userRoleServiceMock = null,
        Mock<IMapper>? mapperMock = null)
    {
        return new UserController(
            (userServiceMock ?? new Mock<IUserService>()).Object,
            (userRoleServiceMock ?? new Mock<IUserRoleService>()).Object,
            (mapperMock ?? new Mock<IMapper>()).Object);
    }

    [Fact]
    public async Task SearchAsync_WithExplicitParameters_ShouldReturnOkAndCallService()
    {
        var expected = new List<UserReadDto> { new() { Id = Guid.NewGuid(), Email = "a@b.c", Username = "test", IsDeviceAdmin = false, IsSysAdmin = false, IsUserAdmin = false } };
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(x => x.SearchAsync("test", 10, 2)).ReturnsAsync(expected);
        var controller = CreateController(userServiceMock: userServiceMock);

        var result = await controller.SearchAsync("test", 10, 2);

        result.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)result.Result!).Value.Should().BeEquivalentTo(expected);
        userServiceMock.Verify(x => x.SearchAsync("test", 10, 2), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithNullParameters_ShouldUseDefaults()
    {
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(x => x.SearchAsync(string.Empty, 50, 0)).ReturnsAsync(new List<UserReadDto>());
        var controller = CreateController(userServiceMock: userServiceMock);

        var result = await controller.SearchAsync(null, null, null);

        result.Result.Should().BeOfType<OkObjectResult>();
        userServiceMock.Verify(x => x.SearchAsync(string.Empty, 50, 0), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WhenLimitTooLarge_ShouldThrowBadRequestException()
    {
        var controller = CreateController();
        Func<Task> act = async () => await controller.SearchAsync("q", 101, 0);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task SearchAsync_WhenLimitNegative_ShouldThrowBadRequestException()
    {
        var controller = CreateController();
        Func<Task> act = async () => await controller.SearchAsync("q", -1, 0);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task SearchAsync_WhenOffsetNegative_ShouldThrowBadRequestException()
    {
        var controller = CreateController();
        Func<Task> act = async () => await controller.SearchAsync("q", 10, -1);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task GetAsync_WhenUserExists_ShouldReturnOk()
    {
        var id = Guid.NewGuid();
        var dto = new UserReadDto { Id = id, Email = "a@b.c", Username = "u", IsDeviceAdmin = false, IsSysAdmin = false, IsUserAdmin = false };
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(x => x.GetAsync(id)).ReturnsAsync(dto);
        var controller = CreateController(userServiceMock: userServiceMock);

        var result = await controller.GetAsync(id);

        result.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)result.Result!).Value.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task GetAsync_WhenUserMissing_ShouldThrowNotFoundException()
    {
        var id = Guid.NewGuid();
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(x => x.GetAsync(id)).ThrowsAsync(new NotFoundException("User", id));
        var controller = CreateController(userServiceMock: userServiceMock);

        Func<Task> act = async () => await controller.GetAsync(id);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task PostAsync_WhenValid_ShouldReturnCreated()
    {
        var write = new UserWriteDto { Email = "a@b.c", UserName = "u", Password = "Password1" };
        var read = new UserReadDto { Id = Guid.NewGuid(), Email = write.Email, Username = write.UserName, IsDeviceAdmin = false, IsSysAdmin = false, IsUserAdmin = false };
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(x => x.PostAsync(write)).ReturnsAsync(read);
        var controller = CreateController(userServiceMock: userServiceMock);

        var result = await controller.PostAsync(write);

        result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task PostAsync_WhenServiceReportsBadRequest_ShouldThrowBadRequestException()
    {
        var write = new UserWriteDto { Email = "a@b.c", UserName = "u", Password = null };
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(x => x.PostAsync(write)).ThrowsAsync(new BadRequestException("Password is required when creating a new user."));
        var controller = CreateController(userServiceMock: userServiceMock);

        Func<Task> act = async () => await controller.PostAsync(write);

        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task PutAsync_WhenExists_ShouldReturnNoContent()
    {
        var id = Guid.NewGuid();
        var write = new UserWriteDto { Email = "a@b.c", UserName = "u" };
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(x => x.PutAsync(id, write)).Returns(Task.CompletedTask);
        var controller = CreateController(userServiceMock: userServiceMock);

        var result = await controller.PutAsync(id, write);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task PutAsync_WhenMissing_ShouldThrowNotFoundException()
    {
        var id = Guid.NewGuid();
        var write = new UserWriteDto { Email = "a@b.c", UserName = "u" };
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(x => x.PutAsync(id, write)).ThrowsAsync(new NotFoundException("User", id));
        var controller = CreateController(userServiceMock: userServiceMock);

        Func<Task> act = async () => await controller.PutAsync(id, write);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenExists_ShouldReturnNoContent()
    {
        var id = Guid.NewGuid();
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(x => x.DeleteAsync(id)).Returns(Task.CompletedTask);
        var controller = CreateController(userServiceMock: userServiceMock);

        var result = await controller.DeleteAsync(id);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenMissing_ShouldThrowNotFoundException()
    {
        var id = Guid.NewGuid();
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(x => x.DeleteAsync(id)).ThrowsAsync(new NotFoundException("User", id));
        var controller = CreateController(userServiceMock: userServiceMock);

        Func<Task> act = async () => await controller.DeleteAsync(id);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetKolcsonzesekAsync_WhenExists_ShouldReturnOk()
    {
        var id = Guid.NewGuid();
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(x => x.GetKolcsonzesekAsync(id)).ReturnsAsync(Array.Empty<KolcsonzesReadDto>());
        var controller = CreateController(userServiceMock: userServiceMock);

        var result = await controller.GetKolcsonzesekAsync(id);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetKolcsonzesekAsync_WhenMissing_ShouldThrowNotFoundException()
    {
        var id = Guid.NewGuid();
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(x => x.GetKolcsonzesekAsync(id)).ThrowsAsync(new NotFoundException("User", id));
        var controller = CreateController(userServiceMock: userServiceMock);

        Func<Task> act = async () => await controller.GetKolcsonzesekAsync(id);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetRendelesekAsync_WhenExists_ShouldReturnOk()
    {
        var id = Guid.NewGuid();
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(x => x.GetRendelesekAsync(id)).ReturnsAsync(Array.Empty<RendelesReadDto>());
        var controller = CreateController(userServiceMock: userServiceMock);

        var result = await controller.GetRendelesekAsync(id);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetRendelesekAsync_WhenMissing_ShouldThrowNotFoundException()
    {
        var id = Guid.NewGuid();
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(x => x.GetRendelesekAsync(id)).ThrowsAsync(new NotFoundException("User", id));
        var controller = CreateController(userServiceMock: userServiceMock);

        Func<Task> act = async () => await controller.GetRendelesekAsync(id);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetFelelossegekAsync_WhenExists_ShouldReturnOk()
    {
        var id = Guid.NewGuid();
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(x => x.GetFelelossegekAsync(id)).ReturnsAsync(Array.Empty<PeldanyReadDto>());
        var controller = CreateController(userServiceMock: userServiceMock);

        var result = await controller.GetFelelossegekAsync(id);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetFelelossegekAsync_WhenMissing_ShouldThrowNotFoundException()
    {
        var id = Guid.NewGuid();
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(x => x.GetFelelossegekAsync(id)).ThrowsAsync(new NotFoundException("User", id));
        var controller = CreateController(userServiceMock: userServiceMock);

        Func<Task> act = async () => await controller.GetFelelossegekAsync(id);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task SetSysAdminRoleAsync_WhenSetTrue_ShouldReturnOk()
    {
        var id = Guid.NewGuid();
        var roleMock = new Mock<IUserRoleService>();
        roleMock.Setup(x => x.SetSysAdminRoleAsync(id, true)).Returns(Task.CompletedTask);
        var controller = CreateController(userRoleServiceMock: roleMock);

        var result = await controller.SetSysAdminRoleAsync(id, true);

        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task SetSysAdminRoleAsync_WhenSetFalse_ShouldReturnOk()
    {
        var id = Guid.NewGuid();
        var roleMock = new Mock<IUserRoleService>();
        roleMock.Setup(x => x.SetSysAdminRoleAsync(id, false)).Returns(Task.CompletedTask);
        var controller = CreateController(userRoleServiceMock: roleMock);

        var result = await controller.SetSysAdminRoleAsync(id, false);

        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task SetUserAdminRoleAsync_WhenSetTrue_ShouldReturnOk()
    {
        var id = Guid.NewGuid();
        var roleMock = new Mock<IUserRoleService>();
        roleMock.Setup(x => x.SetUserAdminRoleAsync(id, true)).Returns(Task.CompletedTask);
        var controller = CreateController(userRoleServiceMock: roleMock);

        var result = await controller.SetUserAdminRoleAsync(id, true);

        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task SetDeviceAdminRoleAsync_WhenSetTrue_ShouldReturnOk()
    {
        var id = Guid.NewGuid();
        var roleMock = new Mock<IUserRoleService>();
        roleMock.Setup(x => x.SetDeviceAdminRoleAsync(id, true)).Returns(Task.CompletedTask);
        var controller = CreateController(userRoleServiceMock: roleMock);

        var result = await controller.SetDeviceAdminRoleAsync(id, true);

        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task CurrentUserAsync_WhenLoggedIn_ShouldReturnOk()
    {
        var user = new User { Id = Guid.NewGuid(), UserName = "u", Email = "e@e.e" };
        var dto = new UserReadDto { Id = user.Id, Username = "u", Email = "e@e.e", IsDeviceAdmin = false, IsSysAdmin = false, IsUserAdmin = false };
        var roleMock = new Mock<IUserRoleService>();
        roleMock.Setup(x => x.GetCurrentUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(x => x.Map<UserReadDto>(user)).Returns(dto);
        var controller = CreateController(userRoleServiceMock: roleMock, mapperMock: mapperMock);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() } };

        var result = await controller.CurrentUserAsync();

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task CurrentUserAsync_WhenNotLoggedIn_ShouldThrowNotLoggedInException()
    {
        var roleMock = new Mock<IUserRoleService>();
        roleMock.Setup(x => x.GetCurrentUserAsync(It.IsAny<ClaimsPrincipal>())).ThrowsAsync(new NotLoggedInException());
        var controller = CreateController(userRoleServiceMock: roleMock);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() } };

        Func<Task> act = async () => await controller.CurrentUserAsync();

        await act.Should().ThrowAsync<NotLoggedInException>();
    }
}
