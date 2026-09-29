using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Services;

namespace VizsgaremekBackend.IntegrationTests.Infrastructure;

public class DatabaseFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer =
        new PostgreSqlBuilder("postgres:latest")
            .WithDatabase("vizsgaremek_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
    
    public DbContextOptions<VizsgaremekContext> DbContextOptions =>
        new DbContextOptionsBuilder<VizsgaremekContext>()
            .UseNpgsql(_postgreSqlContainer.GetConnectionString())
            .LogTo(Console.WriteLine, LogLevel.Information)
            .UseLowerCaseNamingConvention()
            .Options;

    public IMapper Mapper { get; } =
        new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(ServiceNamespaceMarker).Assembly.FullName);
        }).CreateMapper();
    
    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.StopAsync();
    }
}