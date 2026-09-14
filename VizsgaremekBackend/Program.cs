using Microsoft.EntityFrameworkCore;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Services;

namespace VizsgaremekBackend;

public class Program
{
    public static void Main(string[] args)
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
        
        builder.Services.AddAutoMapper(typeof(Program));

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

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}