using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VizsgaremekBackend.Controllers;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.Interfaces;

namespace VizsgaremekBackend.UnitTests.Features;

public class KolcsonzesControllerTests
{
    private static KolcsonzesController CreateController(Mock<IKolcsonzesService>? serviceMock = null)
        => new((serviceMock ?? new Mock<IKolcsonzesService>()).Object);

    [Fact]
    public async Task GetAsync_WhenKolcsonzesExists_ShouldReturnOk()
    {
        //Arrange
        var id = Guid.NewGuid();
        var dto = new KolcsonzesReadDto { Id = id, UserId = Guid.NewGuid(), PeldanyId = Guid.NewGuid(), Date = DateTime.UtcNow };
        var serviceMock = new Mock<IKolcsonzesService>();
        serviceMock.Setup(x => x.GetAsync(id)).ReturnsAsync(dto);

        var controller = CreateController(serviceMock);

        //Act
        var result = await controller.GetAsync(id);

        //Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)result.Result!).Value.Should().BeEquivalentTo(dto);
        serviceMock.Verify(x => x.GetAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WhenKolcsonzesMissing_ShouldThrowNotFoundException()
    {
        //Arrange
        var id = Guid.NewGuid();
        var serviceMock = new Mock<IKolcsonzesService>();
        serviceMock.Setup(x => x.GetAsync(id)).ThrowsAsync(new NotFoundException("Kolcsonzes", id));

        var controller = CreateController(serviceMock);

        //Act
        Func<Task> act = async () => await controller.GetAsync(id);

        //Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task PostAsync_WhenValid_ShouldReturnCreated()
    {
        //Arrange
        var write = new KolcsonzesWriteDto { UserId = Guid.NewGuid(), PeldanyId = Guid.NewGuid(), Date = DateTime.UtcNow };
        var read = new KolcsonzesReadDto { Id = Guid.NewGuid(), UserId = write.UserId, PeldanyId = write.PeldanyId, Date = write.Date };
        var serviceMock = new Mock<IKolcsonzesService>();
        serviceMock.Setup(x => x.PostAsync(write)).ReturnsAsync(read);

        var controller = CreateController(serviceMock);

        //Act
        var result = await controller.PostAsync(write);

        //Assert
        result.Should().BeOfType<CreatedResult>();
        ((CreatedResult)result).Value.Should().BeEquivalentTo(read);
        serviceMock.Verify(x => x.PostAsync(write), Times.Once);
    }

    [Fact]
    public async Task PostAsync_WhenServiceThrowsBadRequest_ShouldPropagate()
    {
        //Arrange
        var write = new KolcsonzesWriteDto { UserId = Guid.NewGuid(), PeldanyId = Guid.NewGuid(), Date = DateTime.UtcNow };
        var serviceMock = new Mock<IKolcsonzesService>();
        serviceMock.Setup(x => x.PostAsync(write)).ThrowsAsync(new BadRequestException("invalid"));

        var controller = CreateController(serviceMock);

        //Act
        Func<Task> act = async () => await controller.PostAsync(write);

        //Assert
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task PutAsync_WhenKolcsonzesExists_ShouldReturnNoContent()
    {
        //Arrange
        var id = Guid.NewGuid();
        var write = new KolcsonzesWriteDto { UserId = Guid.NewGuid(), PeldanyId = Guid.NewGuid(), Date = DateTime.UtcNow };
        var serviceMock = new Mock<IKolcsonzesService>();
        serviceMock.Setup(x => x.PutAsync(id, write)).Returns(Task.CompletedTask);

        var controller = CreateController(serviceMock);

        //Act
        var result = await controller.PutAsync(id, write);

        //Assert
        result.Should().BeOfType<NoContentResult>();
        serviceMock.Verify(x => x.PutAsync(id, write), Times.Once);
    }

    [Fact]
    public async Task PutAsync_WhenKolcsonzesMissing_ShouldThrowNotFoundException()
    {
        //Arrange
        var id = Guid.NewGuid();
        var write = new KolcsonzesWriteDto { UserId = Guid.NewGuid(), PeldanyId = Guid.NewGuid(), Date = DateTime.UtcNow };
        var serviceMock = new Mock<IKolcsonzesService>();
        serviceMock.Setup(x => x.PutAsync(id, write)).ThrowsAsync(new NotFoundException("Kolcsonzes", id));

        var controller = CreateController(serviceMock);

        //Act
        Func<Task> act = async () => await controller.PutAsync(id, write);

        //Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenKolcsonzesExists_ShouldReturnNoContent()
    {
        //Arrange
        var id = Guid.NewGuid();
        var serviceMock = new Mock<IKolcsonzesService>();
        serviceMock.Setup(x => x.DeleteAsync(id)).Returns(Task.CompletedTask);

        var controller = CreateController(serviceMock);

        //Act
        var result = await controller.DeleteAsync(id);

        //Assert
        result.Should().BeOfType<NoContentResult>();
        serviceMock.Verify(x => x.DeleteAsync(id), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenKolcsonzesMissing_ShouldThrowNotFoundException()
    {
        //Arrange
        var id = Guid.NewGuid();
        var serviceMock = new Mock<IKolcsonzesService>();
        serviceMock.Setup(x => x.DeleteAsync(id)).ThrowsAsync(new NotFoundException("Kolcsonzes", id));

        var controller = CreateController(serviceMock);

        //Act
        Func<Task> act = async () => await controller.DeleteAsync(id);

        //Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
