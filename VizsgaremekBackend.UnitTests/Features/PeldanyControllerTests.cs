using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VizsgaremekBackend.Controllers;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.Interfaces;

namespace VizsgaremekBackend.UnitTests.Features;

public class PeldanyControllerTests
{
    private static PeldanyController CreateController(Mock<IPeldanyService>? serviceMock = null)
        => new((serviceMock ?? new Mock<IPeldanyService>()).Object);

    [Fact]
    public async Task GetAsync_WhenPeldanyExists_ShouldReturnOk()
    {
        //Arrange
        var id = Guid.NewGuid();
        var dto = new PeldanyReadDto { Id = id, SerialNumber = "SN", TypeId = Guid.NewGuid(), FelelosId = Guid.NewGuid(), LocationId = Guid.NewGuid() };
        var serviceMock = new Mock<IPeldanyService>();
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
    public async Task GetAsync_WhenPeldanyMissing_ShouldThrowNotFoundException()
    {
        //Arrange
        var id = Guid.NewGuid();
        var serviceMock = new Mock<IPeldanyService>();
        serviceMock.Setup(x => x.GetAsync(id)).ThrowsAsync(new NotFoundException("Peldany", id));

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
        var write = new PeldanyWriteDto { SerialNumber = "SN", AddDate = DateTime.UtcNow, TypeId = Guid.NewGuid(), FelelosId = Guid.NewGuid(), LocationId = Guid.NewGuid() };
        var read = new PeldanyReadDto { Id = Guid.NewGuid(), SerialNumber = write.SerialNumber, TypeId = write.TypeId, FelelosId = write.FelelosId, LocationId = write.LocationId };
        var serviceMock = new Mock<IPeldanyService>();
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
    public async Task PutAsync_WhenPeldanyExists_ShouldReturnNoContent()
    {
        //Arrange
        var id = Guid.NewGuid();
        var write = new PeldanyWriteDto { SerialNumber = "SN", AddDate = DateTime.UtcNow, TypeId = Guid.NewGuid(), FelelosId = Guid.NewGuid(), LocationId = Guid.NewGuid() };
        var serviceMock = new Mock<IPeldanyService>();
        serviceMock.Setup(x => x.PutAsync(id, write)).Returns(Task.CompletedTask);

        var controller = CreateController(serviceMock);

        //Act
        var result = await controller.PutAsync(id, write);

        //Assert
        result.Should().BeOfType<NoContentResult>();
        serviceMock.Verify(x => x.PutAsync(id, write), Times.Once);
    }

    [Fact]
    public async Task PutAsync_WhenPeldanyMissing_ShouldThrowNotFoundException()
    {
        //Arrange
        var id = Guid.NewGuid();
        var write = new PeldanyWriteDto { SerialNumber = "SN", AddDate = DateTime.UtcNow, TypeId = Guid.NewGuid(), FelelosId = Guid.NewGuid(), LocationId = Guid.NewGuid() };
        var serviceMock = new Mock<IPeldanyService>();
        serviceMock.Setup(x => x.PutAsync(id, write)).ThrowsAsync(new NotFoundException("Peldany", id));

        var controller = CreateController(serviceMock);

        //Act
        Func<Task> act = async () => await controller.PutAsync(id, write);

        //Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenPeldanyExists_ShouldReturnNoContent()
    {
        //Arrange
        var id = Guid.NewGuid();
        var serviceMock = new Mock<IPeldanyService>();
        serviceMock.Setup(x => x.DeleteAsync(id)).Returns(Task.CompletedTask);

        var controller = CreateController(serviceMock);

        //Act
        var result = await controller.DeleteAsync(id);

        //Assert
        result.Should().BeOfType<NoContentResult>();
        serviceMock.Verify(x => x.DeleteAsync(id), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenPeldanyMissing_ShouldThrowNotFoundException()
    {
        //Arrange
        var id = Guid.NewGuid();
        var serviceMock = new Mock<IPeldanyService>();
        serviceMock.Setup(x => x.DeleteAsync(id)).ThrowsAsync(new NotFoundException("Peldany", id));

        var controller = CreateController(serviceMock);

        //Act
        Func<Task> act = async () => await controller.DeleteAsync(id);

        //Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
