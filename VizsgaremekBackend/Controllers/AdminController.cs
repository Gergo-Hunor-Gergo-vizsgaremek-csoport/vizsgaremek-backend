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
    [HttpGet(nameof(RecreateDatabase))]
    public async Task<ActionResult> RecreateDatabase()
    {
        await db.Database.EnsureDeletedAsync();

        return await MigrateDatabase();
    }
    
    [HttpGet(nameof(MigrateDatabase))]
    public async Task<ActionResult> MigrateDatabase()
    {
        await db.Database.MigrateAsync();

        await SeedRolesAsync();
        
        return Ok();
    }
    
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