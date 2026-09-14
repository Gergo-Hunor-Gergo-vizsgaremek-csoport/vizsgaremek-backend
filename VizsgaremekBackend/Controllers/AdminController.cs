using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using VizsgaremekBackend.Data;

namespace VizsgaremekBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class AdminController(VizsgaremekContext db) : ControllerBase
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
        
        return Ok();
    }
}