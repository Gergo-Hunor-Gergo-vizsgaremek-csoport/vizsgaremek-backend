using Microsoft.AspNetCore.Mvc;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Services;

namespace VizsgaremekBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class RendelesController(RendelesService rendelesService) : ControllerBase
{
    /// <summary>
    /// Retrieves an order (rendelés) by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the order to retrieve.</param>
    /// <returns>An HTTP 200 OK response containing the requested <see cref="RendelesReadDto"/>.</returns>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when no order exists with the specified <paramref name="id"/>.</exception>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RendelesReadDto>> GetAsync(Guid id)
    {
        return Ok(await rendelesService.GetAsync(id));
    }

    /// <summary>
    /// Creates a new order (rendelés).
    /// </summary>
    /// <param name="rendelesWriteDto">The data used to create the order.</param>
    /// <returns>An HTTP 201 Created response containing the created <see cref="RendelesReadDto"/> and its location.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="rendelesWriteDto"/> is <c>null</c>.</exception>
    [HttpPost]
    public async Task<ActionResult> PostAsync(RendelesWriteDto rendelesWriteDto)
    {
        var createdRendeles = await rendelesService.PostAsync(rendelesWriteDto);
        return Created($"/Rendeles/{createdRendeles.Id}", createdRendeles);
    }

    /// <summary>
    /// Updates an existing order (rendelés).
    /// </summary>
    /// <param name="id">The unique identifier of the order to update.</param>
    /// <param name="rendelesWriteDto">The updated order data.</param>
    /// <returns>An HTTP 204 No Content response when the update succeeds.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="rendelesWriteDto"/> is <c>null</c>.</exception>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when no order exists with the specified <paramref name="id"/>.</exception>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> PutAsync(Guid id, [FromBody] RendelesWriteDto rendelesWriteDto)
    {
        await rendelesService.PutAsync(id, rendelesWriteDto);
        return NoContent();
    }

    /// <summary>
    /// Deletes an order (rendelés) by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the order to delete.</param>
    /// <returns>An HTTP 204 No Content response when the deletion succeeds.</returns>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when no order exists with the specified <paramref name="id"/>.</exception>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        await rendelesService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Assigns item instances (példányok) to an order by creating loans (kölcsönzések) for the provided assignment data.
    /// </summary>
    /// <param name="id">The unique identifier of the order to assign instances to.</param>
    /// <param name="kolcsonzesWriteDtos">The loan data describing each assignment to create.</param>
    /// <returns>An HTTP 201 Created response containing the created <see cref="KolcsonzesReadDto"/> items.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="kolcsonzesWriteDtos"/> is <c>null</c>.</exception>
    /// <exception cref="VizsgaremekBackend.Exceptions.BadRequestException">Thrown when the assignment list is empty or contains a <c>null</c> entry.</exception>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when the order or a referenced instance does not exist.</exception>
    /// <exception cref="VizsgaremekBackend.Exceptions.ConflictException">Thrown when a referenced instance already has an active loan.</exception>
    [HttpPost("{id:guid}/assign")]
    public async Task<ActionResult<KolcsonzesReadDto[]>> AssignPeldanyAsync(Guid id, [FromBody] KolcsonzesWriteDto[] kolcsonzesWriteDtos)
    {
        var createdKolcsonzesek = await rendelesService.AssignPeldanysAsync(id, kolcsonzesWriteDtos);
        return Created($"/Kolcsonzes/{createdKolcsonzesek[0].Id}", createdKolcsonzesek);
    }
}