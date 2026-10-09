using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Models;
using VizsgaremekBackend.Services;

namespace VizsgaremekBackend;

public class Program
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        string[] roleNames = ["SysAdmin", "UserAdmin", "DeviceAdmin"];

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }
        }
    }
    
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<VizsgaremekContext>(options =>
        {
            string? connectionString =
                Environment.GetEnvironmentVariable("VizsgaremekConnectionString");

            options
                .UseNpgsql(connectionString)
                .LogTo(Console.WriteLine, LogLevel.Information)
                .UseLowerCaseNamingConvention();
        });
        
        // Auth
        builder.Services.AddIdentityApiEndpoints<User>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;

                // User settings
                options.User.RequireUniqueEmail = true; 
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<VizsgaremekContext>();
        
        
        builder.Services.AddAutoMapper(_ => {}, typeof(Program).Assembly);

        // Add services to the container.

        builder.Services.Scan(scan => scan
            .FromAssemblyOf<Program>()
            .AddClasses(c => c
                .InNamespaces(typeof(ServiceNamespaceMarker).Namespace!))
            .As(t => t.GetInterfaces().Length != 0
                ? t.GetInterfaces()     // implements interface
                : [t])                  // no interface
            .WithScopedLifetime()
        );

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        
        builder.Services.AddSwaggerGen();

        // Allow frontend to query backend
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy
                    .WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        var app = builder.Build();

        // Seed identity roles on startup
        using (IServiceScope scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            await SeedRolesAsync(roleManager);
        }
        
        app.UseCors("AllowFrontend");
        
        /*
         * initialize EF model on startup
         *
         * first model creation takes ~1500ms
         * consequent uses get model from cache
         * 
         * without this model creation happens during first api call
         */
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider
                .GetRequiredService<VizsgaremekContext>();

            _ = context.Model;
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();

            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapGroup("/auth")
            .MapIdentityApi<User>();


        app.MapControllers();

        app.Run();
    }
}