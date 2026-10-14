using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.IntegrationTests.Infrastructure;
using VizsgaremekBackend.Models;
using VizsgaremekBackend.Services;

namespace VizsgaremekBackend.IntegrationTests.Features.DbAccess;

public class UserServiceTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    [Fact]
    public async Task SearchAsync_WhenUserExists_ReturnsUser()
    {
        //Arrange
        using IServiceScope scope = fixture.CreateScope();
        
        await using VizsgaremekContext dbContext = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();

        User user = new()
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            UserName = "test",
        };
        
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
        
        IMapper mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

        UserService userService = scope.ServiceProvider.GetRequiredService<UserService>();
        
        //Act
        var res = await userService.SearchAsync(user.UserName, 5, 0);

        //Assert
        res.Should().NotBeNull();
        
        res.Should().ContainSingle()
            .Which.Should()
            .BeEquivalentTo(mapper.Map<UserReadDto>(user));
        
    }
    
    
}