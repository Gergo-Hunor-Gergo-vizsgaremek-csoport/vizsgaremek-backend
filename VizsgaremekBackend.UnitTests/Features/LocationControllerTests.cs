using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VizsgaremekBackend.Controllers;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.Services;

namespace VizsgaremekBackend.UnitTests.Features;

public class LocationControllerTests
{
    [Fact]
    public async Task SearchAsync_WhenLimitTooLarge_ShouldThrowBadRequestException()
    {
        var controller = new LocationController(null!);
        Func<Task> act = async () => await controller.SearchAsync("q", 101, 0);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task SearchAsync_WhenLimitNegative_ShouldThrowBadRequestException()
    {
        var controller = new LocationController(null!);
        Func<Task> act = async () => await controller.SearchAsync("q", -1, 0);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task SearchAsync_WhenOffsetNegative_ShouldThrowBadRequestException()
    {
        var controller = new LocationController(null!);
        Func<Task> act = async () => await controller.SearchAsync("q", 10, -1);
        await act.Should().ThrowAsync<BadRequestException>();
    }
}
