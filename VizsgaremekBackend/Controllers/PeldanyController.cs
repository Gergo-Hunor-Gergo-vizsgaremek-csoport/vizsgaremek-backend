using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Interfaces;

namespace VizsgaremekBackend.Controllers;
[ApiController]
[Route("[controller]")]
public class PeldanyController(IPeldanyService peldanyService) : ControllerBase
{
    /// <summary>
    /// Retrieves an item instance (példány) by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the instance to retrieve.</param>
    /// <returns>An HTTP 200 OK response containing the requested <see cref="PeldanyReadDto"/>.</returns>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when no instance exists with the specified <paramref name="id"/>.</exception>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PeldanyReadDto>> GetAsync(Guid id)
    {
        return Ok(await peldanyService.GetAsync(id));
    }
    
    [HttpGet("by-typeId/{id:guid}")]
    public async Task<ActionResult<PeldanyReadDto[]>> GetByTypeIdAsync(Guid id)
    {
        return Ok(await peldanyService.GetByTypeIdAsync(id));
    }

    /// <summary>
    /// Creates a new item instance (példány).
    /// </summary>
    /// <param name="peldanyWriteDto">The data used to create the instance.</param>
    /// <returns>An HTTP 201 Created response containing the created <see cref="PeldanyReadDto"/> and its location.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="peldanyWriteDto"/> is <c>null</c>.</exception>
    [HttpPost]
    public async Task<ActionResult> PostAsync([FromBody] PeldanyWriteDto peldanyWriteDto)
    {
        PeldanyReadDto res = await peldanyService.PostAsync(peldanyWriteDto);

        return Created($"/Peldany/{res.Id}", res);
    }

    /// <summary>
    /// Updates an existing item instance (példány).
    /// </summary>
    /// <param name="id">The unique identifier of the instance to update.</param>
    /// <param name="peldanyWriteDto">The updated instance data.</param>
    /// <returns>An HTTP 204 No Content response when the update succeeds.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="peldanyWriteDto"/> is <c>null</c>.</exception>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when no instance exists with the specified <paramref name="id"/>.</exception>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> PutAsync(Guid id, [FromBody] PeldanyWriteDto peldanyWriteDto)
    {
        await peldanyService.PutAsync(id, peldanyWriteDto);
        return NoContent();
    }

    /// <summary>
    /// Deletes an item instance (példány) by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the instance to delete.</param>
    /// <returns>An HTTP 204 No Content response when the deletion succeeds.</returns>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when no instance exists with the specified <paramref name="id"/>.</exception>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        await peldanyService.DeleteAsync(id);
        return NoContent();
    }
}