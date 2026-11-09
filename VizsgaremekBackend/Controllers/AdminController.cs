using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using VizsgaremekBackend.Data;

namespace VizsgaremekBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class AdminController(VizsgaremekContext db, RoleManager<IdentityRole<Guid>> roleManager) : ControllerBase
{
    /// <summary>
    /// Deletes the database (if it exists), recreates it by applying migrations, and seeds default roles.
    /// </summary>
    /// <returns>An HTTP 200 OK result when the database has been recreated and seeded successfully.</returns>
    [HttpGet(nameof(RecreateDatabase))]
    public async Task<ActionResult> RecreateDatabase()
    {
        await db.Database.EnsureDeletedAsync();

        return await MigrateDatabase();
    }

    /// <summary>
    /// Applies any pending database migrations and seeds default roles.
    /// </summary>
    /// <returns>An HTTP 200 OK result when migrations and role seeding have completed successfully.</returns>
    [HttpGet(nameof(MigrateDatabase))]
    public async Task<ActionResult> MigrateDatabase()
    {
        await db.Database.MigrateAsync();

        await SeedRolesAsync();

        return Ok();
    }

    /// <summary>
    /// Ensures that the default application roles (<c>SysAdmin</c>, <c>UserAdmin</c>, <c>DeviceAdmin</c>) exist,
    /// creating any that are missing.
    /// </summary>
    /// <returns>A task that represents the asynchronous seeding operation.</returns>
    [NonAction]
    public async Task SeedRolesAsync()
    {
        string[] roleNames = ["SysAdmin", "UserAdmin", "DeviceAdmin"];

        foreach (string roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }
        }
    }
}