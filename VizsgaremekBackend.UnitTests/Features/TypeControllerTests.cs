using FluentAssertions;
using VizsgaremekBackend.Controllers;
using VizsgaremekBackend.Exceptions;

namespace VizsgaremekBackend.UnitTests.Features;

public class TypeControllerTests
{
    [Fact]
    public async Task SearchAsync_WhenLimitTooLarge_ShouldThrowBadRequestException()
    {
        var controller = new TypeController(null!);
        Func<Task> act = async () => await controller.SearchAsync("q", 101, 0);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task SearchAsync_WhenLimitNegative_ShouldThrowBadRequestException()
    {
        var controller = new TypeController(null!);
        Func<Task> act = async () => await controller.SearchAsync("q", -1, 0);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task SearchAsync_WhenOffsetNegative_ShouldThrowBadRequestException()
    {
        var controller = new TypeController(null!);
        Func<Task> act = async () => await controller.SearchAsync("q", 10, -1);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task GetPeldanysAsync_WhenLimitTooLarge_ShouldThrowBadRequestException()
    {
        var controller = new TypeController(null!);
        Func<Task> act = async () => await controller.GetPeldanysAsync(Guid.NewGuid(), 101, 0);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task GetPeldanysAsync_WhenLimitNegative_ShouldThrowBadRequestException()
    {
        var controller = new TypeController(null!);
        Func<Task> act = async () => await controller.GetPeldanysAsync(Guid.NewGuid(), -1, 0);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task GetPeldanysAsync_WhenOffsetNegative_ShouldThrowBadRequestException()
    {
        var controller = new TypeController(null!);
        Func<Task> act = async () => await controller.GetPeldanysAsync(Guid.NewGuid(), 10, -1);
        await act.Should().ThrowAsync<BadRequestException>();
    }
}
