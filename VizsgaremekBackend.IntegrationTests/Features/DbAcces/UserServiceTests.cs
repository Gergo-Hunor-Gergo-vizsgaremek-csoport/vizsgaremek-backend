using FluentAssertions;
using Microsoft.AspNetCore.Identity;
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
        await using VizsgaremekContext dbContext = fixture.CreateDbContext();

        User user = new()
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            UserName = "test",
        };
        
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        UserManager<User> userManager = fixture.UserManager;

        UserService userService = new(dbContext, fixture.Mapper, userManager);
        
        //Act
        var res = await userService.SearchAsync(user.UserName, 5, 0);

        //Assert
        res.Should().NotBeNull();
        
        res.Should().ContainSingle()
            .Which.Should()
            .BeEquivalentTo(fixture.Mapper.Map<UserReadDto>(user));
        
    }
    
    
}