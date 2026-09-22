using Microsoft.EntityFrameworkCore;
using VizsgaremekBackend.Models;
using Type = VizsgaremekBackend.Models.Type;

namespace VizsgaremekBackend.Data;

public class VizsgaremekContext(DbContextOptions<VizsgaremekContext> options) : DbContext(options)
{
    public DbSet<Kolcsonzes> Kolcsonzess { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<Peldany> Peldanys { get; set; }
    public DbSet<Type> Types { get; set; }
    public DbSet<User> Users { get; set; }
}