using Microsoft.AspNetCore.Mvc;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.Services;

namespace VizsgaremekBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class LogController(LogService logService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<LogReadDto>>> SearchAsync(string? q, int? limit, int? offset)
    {
        const int maxResults = 100;
        const int defaultLimit = 50;

        if (limit is > maxResults) throw new BadRequestException($"Limit may not be more than {maxResults}.");
        if (limit is < 0) throw new BadRequestException("Limit must be greater than or equal to 0.");
        if (offset is < 0) throw new BadRequestException("Offset must be greater than or equal to 0.");

        limit ??= defaultLimit;
        offset ??= 0;
        q ??= string.Empty;

        List<LogReadDto> result = await logService.SearchAsync(q, limit.Value, offset.Value);
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
        LogReadDto res = await logService.PostAsync(logWriteDto);
        return Created($"/Log/{res.Id}", res);
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