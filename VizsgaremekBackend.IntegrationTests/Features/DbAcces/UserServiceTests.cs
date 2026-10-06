using FluentAssertions;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.IntegrationTests.Infrastructure;
using VizsgaremekBackend.Models;
using VizsgaremekBackend.Services;

namespace VizsgaremekBackend.IntegrationTests.Features.DbAcces;

public class UserServiceTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    [Fact]
    public async Task SearchAsync_WhenUserExists_ReturnsUser()
    {
        //Arrange
        await using VizsgaremekContext dbContext = new(fixture.DbContextOptions);

        User user = new()
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            Name = "test",
            IsDeviceAdmin = false,
            IsSysAdmin = false,
            IsUserAdmin = false,
        };
        
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        UserService userService = new(dbContext, fixture.Mapper);
        
        //Act
        var res = await userService.SearchAsync(user.Name, 5, 0);

        //Assert
        res.Should().NotBeNull();
        
        res.Should().ContainSingle()
            .Which.Should()
            .BeEquivalentTo(fixture.Mapper.Map<UserReadDto>(user));
        
    }
    
    
}