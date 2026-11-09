using Microsoft.AspNetCore.Mvc;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.Services;

namespace VizsgaremekBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class LocationController(LocationService locationService) : ControllerBase
{
    /// <summary>
    /// Searches for locations by name using optional pagination.
    /// </summary>
    /// <param name="q">The optional search query matched against location names. Defaults to an empty string when <c>null</c>.</param>
    /// <param name="limit">The maximum number of results to return. Defaults to 50 and may not exceed 100.</param>
    /// <param name="offset">The number of results to skip for pagination. Defaults to 0.</param>
    /// <returns>An HTTP 200 OK response containing the matching <see cref="LocationReadDto"/> items.</returns>
    /// <exception cref="BadRequestException">Thrown when <paramref name="limit"/> is greater than 100, or when <paramref name="limit"/> or <paramref name="offset"/> is negative.</exception>
    [HttpGet]
    public async Task<ActionResult<List<LocationReadDto>>> SearchAsync(string? q, int? limit, int? offset)
    {
        const int maxResults = 100;
        const int defaultLimit = 50;

        if (limit is > maxResults) throw new BadRequestException($"Limit may not be more than {maxResults}.");
        if (limit is < 0) throw new BadRequestException("Limit must be greater than or equal to 0.");
        if (offset is < 0) throw new BadRequestException("Offset must be greater than or equal to 0.");

        limit ??= defaultLimit;
        offset ??= 0;
        q ??= string.Empty;

        List<LocationReadDto> result = await locationService.SearchAsync(q, limit.Value, offset.Value);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a location by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the location to retrieve.</param>
    /// <returns>An HTTP 200 OK response containing the requested <see cref="LocationReadDto"/>.</returns>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when no location exists with the specified <paramref name="id"/>.</exception>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LocationReadDto>> GetAsync(Guid id)
    {
        return Ok(await locationService.GetAsync(id));
    }

    /// <summary>
    /// Creates a new location.
    /// </summary>
    /// <param name="locationWriteDto">The data used to create the location.</param>
    /// <returns>An HTTP 201 Created response containing the created <see cref="LocationReadDto"/> and its location.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="locationWriteDto"/> is <c>null</c>.</exception>
    [HttpPost]
    public async Task<ActionResult> PostAsync([FromBody] LocationWriteDto locationWriteDto)
    {
        LocationReadDto res = await locationService.PostAsync(locationWriteDto);
        return Created($"/Location/{res.Id}", res);
    }

    /// <summary>
    /// Updates an existing location.
    /// </summary>
    /// <param name="id">The unique identifier of the location to update.</param>
    /// <param name="locationWriteDto">The updated location data.</param>
    /// <returns>An HTTP 204 No Content response when the update succeeds.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="locationWriteDto"/> is <c>null</c>.</exception>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when no location exists with the specified <paramref name="id"/>.</exception>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> PutAsync(Guid id, [FromBody] LocationWriteDto locationWriteDto)
    {
        await locationService.PutAsync(id, locationWriteDto);
        return NoContent();
    }

    /// <summary>
    /// Deletes a location by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the location to delete.</param>
    /// <returns>An HTTP 204 No Content response when the deletion succeeds.</returns>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when no location exists with the specified <paramref name="id"/>.</exception>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        await locationService.DeleteAsync(id);
        return NoContent();
    }
}