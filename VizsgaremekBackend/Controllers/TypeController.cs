using Microsoft.AspNetCore.Mvc;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.Services;

namespace VizsgaremekBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class TypeController(TypeService typeService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedDto<TypeReadDto>>> SearchAsync(string? q, int? limit, int? offset)
    {
        const int maxResults = 100;
        const int defaultLimit = 50;

        if (limit is > maxResults) throw new BadRequestException($"Limit may not be more than {maxResults}.");
        if (limit is < 0) throw new BadRequestException("Limit must be greater than or equal to 0.");
        if (offset is < 0) throw new BadRequestException("Offset must be greater than or equal to 0.");

        limit ??= defaultLimit;
        offset ??= 0;
        q ??= string.Empty;

        PagedDto<TypeReadDto> result = await typeService.SearchAsync(q, limit.Value, offset.Value);
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
        TypeReadDto res = await typeService.PostAsync(typeWriteDto);
        return Created($"/Type/{res.Id}", res);
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

    [HttpGet("{id:guid}/peldanyok")]
    public async Task<ActionResult<PeldanyReadDto[]>> GetPeldanysAsync(Guid id, int? limit, int? offset)
    {
        const int maxResults = 100;
        const int defaultLimit = 50;

        if (limit is > maxResults) throw new BadRequestException($"Limit may not be more than {maxResults}.");
        if (limit is < 0) throw new BadRequestException("Limit must be greater than or equal to 0.");
        if (offset is < 0) throw new BadRequestException("Offset must be greater than or equal to 0.");

        limit ??= defaultLimit;
        offset ??= 0;

        return Ok(await typeService.GetPeldanys(id, limit.Value, offset.Value));
    }
}