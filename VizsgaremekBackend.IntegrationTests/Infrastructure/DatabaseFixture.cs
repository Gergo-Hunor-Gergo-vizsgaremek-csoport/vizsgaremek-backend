using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.PostgreSql;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Models;
using VizsgaremekBackend.Services;

namespace VizsgaremekBackend.IntegrationTests.Infrastructure;

public class DatabaseFixture : IAsyncLifetime
{
    public CustomWebApplicationFactory Factory { get; private set; } = null!;
    /*
    public HttpClient Client { get; private set; } = null!;
    public IMapper Mapper { get; private set; } = null!;
    public UserManager<User> UserManager { get; private set; } = null!;
    */

    public async Task InitializeAsync()
    {
        Factory = new CustomWebApplicationFactory();
        await Factory.InitializeAsync();

        /*
        // Ensure host is instantiated by creating a client
        Client = Factory.CreateClient();
        */

        // Apply migrations to prepare schema
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        await context.Database.MigrateAsync();
        
        /*
        UserManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        // Initialize AutoMapper
        Mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(ServiceNamespaceMarker).Assembly.FullName);
        }, new NullLoggerFactory()).CreateMapper();
        */
    }

    // Helper method to resolve scoped services like UserManager<TUser> or DbContext in tests
    public IServiceScope CreateScope()
    {
        return Factory.Services.CreateScope();
    }
    
    

    /*
    public VizsgaremekContext CreateDbContext()
    {
        using IServiceScope scope = Factory.Services.CreateScope();
        VizsgaremekContext context = scope.ServiceProvider.GetRequiredService<VizsgaremekContext>();
        return context;
    }
    */

    public async Task DisposeAsync()
    {
        //Client.Dispose();
        await Factory.DisposeAsync();
    }
}