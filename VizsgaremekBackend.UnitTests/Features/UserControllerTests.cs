using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VizsgaremekBackend.Controllers;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Interfaces;

namespace VizsgaremekBackend.UnitTests.Features;

public class UserControllerTests
{
    [Fact]
    public async Task SearchAsync_WithValidParameters_ShouldReturnOk()
    {
        //Arrange
        List<UserReadDto> expectedResult = [
            new()
            {
                Id = new Guid(),
                Email = "user@example.com",
                Name = "test",
                IsDeviceAdmin = false,
                IsSysAdmin = false,
                IsUserAdmin = false,
            },
        ];
        
        Mock<IUserService> userServiceMock = new();
        
        userServiceMock
            .Setup(x => x.SearchAsync("test", It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(expectedResult);
        
        UserController controller = new(userServiceMock.Object);

        //Act
        
        var result = await controller.SearchAsync("test", null, null);
        
        //Assert
        
        result.Result.Should().BeOfType<OkObjectResult>();
        
        result.Value.Should().BeEquivalentTo(expectedResult);

        userServiceMock.Verify(x =>
            x.SearchAsync("test", It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        
    }
}