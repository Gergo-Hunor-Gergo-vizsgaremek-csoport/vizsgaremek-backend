using Microsoft.EntityFrameworkCore;

namespace VizsgaremekBackend.Data;

public class VizsgaremekContext(DbContextOptions<VizsgaremekContext> options) : DbContext(options)
{
}