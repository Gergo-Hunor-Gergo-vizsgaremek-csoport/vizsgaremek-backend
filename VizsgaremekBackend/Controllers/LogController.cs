using Microsoft.AspNetCore.Mvc;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.Services;

namespace VizsgaremekBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class LogController(LogService logService) : ControllerBase
{
    /// <summary>
    /// Searches for log entries by type using optional pagination.
    /// </summary>
    /// <param name="q">The optional search query matched against log entry types. Defaults to an empty string when <c>null</c>.</param>
    /// <param name="limit">The maximum number of results to return. Defaults to 50 and may not exceed 100.</param>
    /// <param name="offset">The number of results to skip for pagination. Defaults to 0.</param>
    /// <returns>An HTTP 200 OK response containing the matching <see cref="LogReadDto"/> items.</returns>
    /// <exception cref="BadRequestException">Thrown when <paramref name="limit"/> is greater than 100, or when <paramref name="limit"/> or <paramref name="offset"/> is negative.</exception>
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

    /// <summary>
    /// Retrieves a log entry by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the log entry to retrieve.</param>
    /// <returns>An HTTP 200 OK response containing the requested <see cref="LogReadDto"/>.</returns>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when no log entry exists with the specified <paramref name="id"/>.</exception>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LogReadDto>> GetAsync(Guid id)
    {
        return Ok(await logService.GetAsync(id));
    }

    /// <summary>
    /// Creates a new log entry.
    /// </summary>
    /// <param name="logWriteDto">The data used to create the log entry.</param>
    /// <returns>An HTTP 201 Created response containing the created <see cref="LogReadDto"/> and its location.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logWriteDto"/> is <c>null</c>.</exception>
    [HttpPost]
    public async Task<ActionResult> PostAsync([FromBody] LogWriteDto logWriteDto)
    {
        LogReadDto res = await logService.PostAsync(logWriteDto);
        return Created($"/Log/{res.Id}", res);
    }

    /// <summary>
    /// Updates an existing log entry.
    /// </summary>
    /// <param name="id">The unique identifier of the log entry to update.</param>
    /// <param name="logWriteDto">The updated log entry data.</param>
    /// <returns>An HTTP 204 No Content response when the update succeeds.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logWriteDto"/> is <c>null</c>.</exception>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when no log entry exists with the specified <paramref name="id"/>.</exception>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> PutAsync(Guid id, [FromBody] LogWriteDto logWriteDto)
    {
        await logService.PutAsync(id, logWriteDto);
        return NoContent();
    }

    /// <summary>
    /// Deletes a log entry by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the log entry to delete.</param>
    /// <returns>An HTTP 204 No Content response when the deletion succeeds.</returns>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when no log entry exists with the specified <paramref name="id"/>.</exception>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        await logService.DeleteAsync(id);
        return NoContent();
    }
}