using Microsoft.AspNetCore.Mvc;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Services;

namespace VizsgaremekBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class TypeController(TypeService typeService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TypeReadDto>>> SearchAsync(string q, int? limit, int? offset)
    {
        const int maxResults = 100;
        const int defaultLimit = 50;
        
        if (limit > maxResults) return BadRequest($"Limit may not be more than {maxResults}");
        limit ??= defaultLimit;

        offset ??= 0;
        q ??= "";
        
        List<TypeReadDto> result = await typeService.SearchAsync(q, (int)limit, (int)offset);
        return Ok(result);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TypeReadDto>> GetAsync(Guid id)
    {
        return Ok(await typeService.GetAsync(id));
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync([FromBody] TypeWriteDto typeWriteDto)
    {
        await typeService.PostAsync(typeWriteDto);

        return Created();
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> PutAsync(Guid id, [FromBody] TypeWriteDto typeWriteDto)
    {
        await typeService.PutAsync(id, typeWriteDto);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        await typeService.DeleteAsync(id);
        return NoContent();
    }
    
}