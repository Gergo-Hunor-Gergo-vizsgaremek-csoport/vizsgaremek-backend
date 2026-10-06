using Microsoft.AspNetCore.Mvc;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Services;

namespace VizsgaremekBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class LocationController(LocationService locationService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<LocationReadDto>>> SearchAsync(string? q, int? limit, int? offset)
    {
        const int maxResults = 100;
        const int defaultLimit = 50;
        
        if (limit > maxResults) return BadRequest($"Limit may not be more than {maxResults}");
        limit ??= defaultLimit;

        offset ??= 0;
        q ??= "";
        
        List<LocationReadDto> result = await locationService.SearchAsync(q, (int)limit, (int)offset);
        return Ok(result);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LocationReadDto>> GetAsync(Guid id)
    {
        return Ok(await locationService.GetAsync(id));
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync([FromBody] LocationWriteDto locationWriteDto)
    {
        await locationService.PostAsync(locationWriteDto);

        return Created();
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> PutAsync(Guid id, [FromBody] LocationWriteDto locationWriteDto)
    {
        await locationService.PutAsync(id, locationWriteDto);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        await locationService.DeleteAsync(id);
        return NoContent();
    }
    
}