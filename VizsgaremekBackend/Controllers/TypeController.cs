using Microsoft.AspNetCore.Mvc;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.Services;

namespace VizsgaremekBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class TypeController(TypeService typeService) : ControllerBase
{
    /// <summary>
    /// Searches for types by name using optional pagination.
    /// </summary>
    /// <param name="q">The optional search query matched against type names. Defaults to an empty string when <c>null</c>.</param>
    /// <param name="limit">The maximum number of results to return. Defaults to 50 and may not exceed 100.</param>
    /// <param name="offset">The number of results to skip for pagination. Defaults to 0.</param>
    /// <returns>An HTTP 200 OK response containing a paged result of <see cref="TypeReadDto"/> items.</returns>
    /// <exception cref="BadRequestException">Thrown when <paramref name="limit"/> is greater than 100, or when <paramref name="limit"/> or <paramref name="offset"/> is negative.</exception>
    [HttpGet]
    public async Task<ActionResult<TypeReadDto[]>> SearchAsync(string? q, int? limit, int? offset)
    {
        const int maxResults = 100;
        const int defaultLimit = 50;

        if (limit is > maxResults) throw new BadRequestException($"Limit may not be more than {maxResults}.");
        if (limit is < 0) throw new BadRequestException("Limit must be greater than or equal to 0.");
        if (offset is < 0) throw new BadRequestException("Offset must be greater than or equal to 0.");

        limit ??= defaultLimit;
        offset ??= 0;
        q ??= string.Empty;

        TypeReadDto[] result = await typeService.SearchAsync(q, limit.Value, offset.Value);
        return Ok(result);
    }

    [HttpGet("count")]
    public async Task<ActionResult<int>> SearchCountAsync(string? q)
    {
        q ??= string.Empty;

        int result = (await typeService.SearchCountAsync(q));
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a type by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the type to retrieve.</param>
    /// <returns>An HTTP 200 OK response containing the requested <see cref="TypeReadDto"/>.</returns>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when no type exists with the specified <paramref name="id"/>.</exception>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TypeReadDto>> GetAsync(Guid id)
    {
        return Ok(await typeService.GetAsync(id));
    }

    /// <summary>
    /// Creates a new type.
    /// </summary>
    /// <param name="typeWriteDto">The data used to create the type.</param>
    /// <returns>An HTTP 201 Created response containing the created <see cref="TypeReadDto"/> and its location.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="typeWriteDto"/> is <c>null</c>.</exception>
    [HttpPost]
    public async Task<ActionResult> PostAsync([FromBody] TypeWriteDto typeWriteDto)
    {
        TypeReadDto res = await typeService.PostAsync(typeWriteDto);
        return Created($"/Type/{res.Id}", res);
    }

    /// <summary>
    /// Updates an existing type.
    /// </summary>
    /// <param name="id">The unique identifier of the type to update.</param>
    /// <param name="typeWriteDto">The updated type data.</param>
    /// <returns>An HTTP 204 No Content response when the update succeeds.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="typeWriteDto"/> is <c>null</c>.</exception>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when no type exists with the specified <paramref name="id"/>.</exception>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> PutAsync(Guid id, [FromBody] TypeWriteDto typeWriteDto)
    {
        await typeService.PutAsync(id, typeWriteDto);
        return NoContent();
    }

    /// <summary>
    /// Deletes a type by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the type to delete.</param>
    /// <returns>An HTTP 204 No Content response when the deletion succeeds.</returns>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when no type exists with the specified <paramref name="id"/>.</exception>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        await typeService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Retrieves available item instances (példányok) for a type using optional pagination.
    /// </summary>
    /// <param name="id">The unique identifier of the type whose instances are requested.</param>
    /// <param name="limit">The maximum number of instances to return. Defaults to 50 and may not exceed 100.</param>
    /// <param name="offset">The number of instances to skip for pagination. Defaults to 0.</param>
    /// <returns>An HTTP 200 OK response containing available <see cref="PeldanyReadDto"/> items for the specified type.</returns>
    /// <exception cref="BadRequestException">Thrown when <paramref name="limit"/> is greater than 100, or when <paramref name="limit"/> or <paramref name="offset"/> is negative.</exception>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when no type exists with the specified <paramref name="id"/>.</exception>
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