using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VizsgaremekBackend.Controllers;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.Interfaces;

namespace VizsgaremekBackend.UnitTests.Features;

public class RendelesControllerTests
{
    private static RendelesController CreateController(Mock<IRendelesService>? serviceMock = null)
        => new((serviceMock ?? new Mock<IRendelesService>()).Object);

    [Fact]
    public async Task GetAsync_WhenRendelesExists_ShouldReturnOk()
    {
        //Arrange
        var id = Guid.NewGuid();
        var dto = new RendelesReadDto { Id = id, UserId = Guid.NewGuid(), TypeId = Guid.NewGuid(), Date = DateTime.UtcNow, Quantity = 1, CompletedQuantity = 0 };
        var serviceMock = new Mock<IRendelesService>();
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
    public async Task GetAsync_WhenRendelesMissing_ShouldThrowNotFoundException()
    {
        //Arrange
        var id = Guid.NewGuid();
        var serviceMock = new Mock<IRendelesService>();
        serviceMock.Setup(x => x.GetAsync(id)).ThrowsAsync(new NotFoundException("Rendeles", id));

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
        var write = new RendelesWriteDto { UserId = Guid.NewGuid(), TypeId = Guid.NewGuid(), Date = DateTime.UtcNow, Quantity = 1, CompletedQuantity = 0 };
        var read = new RendelesReadDto { Id = Guid.NewGuid(), UserId = write.UserId, TypeId = write.TypeId, Date = write.Date, Quantity = write.Quantity, CompletedQuantity = write.CompletedQuantity };
        var serviceMock = new Mock<IRendelesService>();
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
    public async Task PutAsync_WhenRendelesExists_ShouldReturnNoContent()
    {
        //Arrange
        var id = Guid.NewGuid();
        var write = new RendelesWriteDto { UserId = Guid.NewGuid(), TypeId = Guid.NewGuid(), Date = DateTime.UtcNow, Quantity = 1, CompletedQuantity = 0 };
        var serviceMock = new Mock<IRendelesService>();
        serviceMock.Setup(x => x.PutAsync(id, write)).Returns(Task.CompletedTask);

        var controller = CreateController(serviceMock);

        //Act
        var result = await controller.PutAsync(id, write);

        //Assert
        result.Should().BeOfType<NoContentResult>();
        serviceMock.Verify(x => x.PutAsync(id, write), Times.Once);
    }

    [Fact]
    public async Task PutAsync_WhenRendelesMissing_ShouldThrowNotFoundException()
    {
        //Arrange
        var id = Guid.NewGuid();
        var write = new RendelesWriteDto { UserId = Guid.NewGuid(), TypeId = Guid.NewGuid(), Date = DateTime.UtcNow, Quantity = 1, CompletedQuantity = 0 };
        var serviceMock = new Mock<IRendelesService>();
        serviceMock.Setup(x => x.PutAsync(id, write)).ThrowsAsync(new NotFoundException("Rendeles", id));

        var controller = CreateController(serviceMock);

        //Act
        Func<Task> act = async () => await controller.PutAsync(id, write);

        //Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenRendelesExists_ShouldReturnNoContent()
    {
        //Arrange
        var id = Guid.NewGuid();
        var serviceMock = new Mock<IRendelesService>();
        serviceMock.Setup(x => x.DeleteAsync(id)).Returns(Task.CompletedTask);

        var controller = CreateController(serviceMock);

        //Act
        var result = await controller.DeleteAsync(id);

        //Assert
        result.Should().BeOfType<NoContentResult>();
        serviceMock.Verify(x => x.DeleteAsync(id), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenRendelesMissing_ShouldThrowNotFoundException()
    {
        //Arrange
        var id = Guid.NewGuid();
        var serviceMock = new Mock<IRendelesService>();
        serviceMock.Setup(x => x.DeleteAsync(id)).ThrowsAsync(new NotFoundException("Rendeles", id));

        var controller = CreateController(serviceMock);

        //Act
        Func<Task> act = async () => await controller.DeleteAsync(id);

        //Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task AssignPeldanyAsync_WhenValid_ShouldReturnCreated()
    {
        //Arrange
        var rendelesId = Guid.NewGuid();
        var write = new[] { new KolcsonzesWriteDto { UserId = Guid.NewGuid(), PeldanyId = Guid.NewGuid(), Date = DateTime.UtcNow } };
        var read = new[] { new KolcsonzesReadDto { Id = Guid.NewGuid(), UserId = write[0].UserId, PeldanyId = write[0].PeldanyId, Date = write[0].Date } };
        var serviceMock = new Mock<IRendelesService>();
        serviceMock.Setup(x => x.AssignPeldanysAsync(rendelesId, write)).ReturnsAsync(read);

        var controller = CreateController(serviceMock);

        //Act
        var result = await controller.AssignPeldanyAsync(rendelesId, write);

        //Assert
        result.Result.Should().BeOfType<CreatedResult>();
        ((CreatedResult)result.Result!).Value.Should().BeEquivalentTo(read);
        serviceMock.Verify(x => x.AssignPeldanysAsync(rendelesId, write), Times.Once);
    }

    [Fact]
    public async Task AssignPeldanyAsync_WhenServiceThrowsConflict_ShouldPropagate()
    {
        //Arrange
        var rendelesId = Guid.NewGuid();
        var write = new[] { new KolcsonzesWriteDto { UserId = Guid.NewGuid(), PeldanyId = Guid.NewGuid(), Date = DateTime.UtcNow } };
        var serviceMock = new Mock<IRendelesService>();
        serviceMock.Setup(x => x.AssignPeldanysAsync(rendelesId, write)).ThrowsAsync(new ConflictException("already assigned"));

        var controller = CreateController(serviceMock);

        //Act
        Func<Task> act = async () => await controller.AssignPeldanyAsync(rendelesId, write);

        //Assert
        await act.Should().ThrowAsync<ConflictException>();
    }
}
