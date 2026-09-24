using Microsoft.AspNetCore.Mvc;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Services;

namespace VizsgaremekBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(UserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<UserReadDto>>> SearchAsync(string q, int? limit, int? offset)
    {
        const int maxResults = 100;
        const int defaultLimit = 50;
        
        if (limit > maxResults) return BadRequest($"Limit may not be more than {maxResults}");
        limit ??= defaultLimit;

        offset ??= 0;
        q ??= "";
        
        List<UserReadDto> result = await userService.SearchAsync(q, (int)limit, (int)offset);
        return Ok(result);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserReadDto>> GetAsync(Guid id)
    {
        return Ok(await userService.GetAsync(id));
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync([FromBody] UserWriteDto userWriteDto)
    {
        await userService.PostAsync(userWriteDto);

        return Created();
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> PutAsync(Guid id, [FromBody] UserWriteDto userWriteDto)
    {
        await userService.PutAsync(id, userWriteDto);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        await userService.DeleteAsync(id);
        return NoContent();
    }
    
}