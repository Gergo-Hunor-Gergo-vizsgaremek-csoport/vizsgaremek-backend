using Microsoft.AspNetCore.Mvc;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Services;

namespace VizsgaremekBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class LogController(LogService logService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<LogReadDto>>> SearchAsync(string q, int? limit, int? offset)
    {
        const int maxResults = 100;
        const int defaultLimit = 50;
        
        if (limit > maxResults) return BadRequest($"Limit may not be more than {maxResults}");
        limit ??= defaultLimit;

        offset ??= 0;
        q ??= "";
        
        List<LogReadDto> result = await logService.SearchAsync(q, (int)limit, (int)offset);
        return Ok(result);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LogReadDto>> GetAsync(Guid id)
    {
        return Ok(await logService.GetAsync(id));
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync([FromBody] LogWriteDto logWriteDto)
    {
        await logService.PostAsync(logWriteDto);

        return Created();
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> PutAsync(Guid id, [FromBody] LogWriteDto logWriteDto)
    {
        await logService.PutAsync(id, logWriteDto);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        await logService.DeleteAsync(id);
        return NoContent();
    }
    
}