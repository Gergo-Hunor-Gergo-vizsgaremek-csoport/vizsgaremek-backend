using Microsoft.AspNetCore.Mvc;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Services;

namespace VizsgaremekBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class KolcsonzesController(KolcsonzesService kolcsonzesService) : ControllerBase
{
    /// <summary>
    /// Retrieves a loan (kölcsönzés) by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the loan to retrieve.</param>
    /// <returns>An HTTP 200 OK response containing the requested <see cref="KolcsonzesReadDto"/>.</returns>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when no loan exists with the specified <paramref name="id"/>.</exception>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<KolcsonzesReadDto>> GetAsync(Guid id)
    {
        return Ok(await kolcsonzesService.GetAsync(id));
    }

    /// <summary>
    /// Creates a new loan (kölcsönzés).
    /// </summary>
    /// <param name="kolcsonzesWriteDto">The data used to create the loan.</param>
    /// <returns>An HTTP 201 Created response containing the created <see cref="KolcsonzesReadDto"/> and its location.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="kolcsonzesWriteDto"/> is <c>null</c>.</exception>
    [HttpPost]
    public async Task<ActionResult> PostAsync([FromBody] KolcsonzesWriteDto kolcsonzesWriteDto)
    {
        KolcsonzesReadDto res = await kolcsonzesService.PostAsync(kolcsonzesWriteDto);

        return Created($"/Kolcsonzes/{res.Id}", res);
    }

    /// <summary>
    /// Updates an existing loan (kölcsönzés).
    /// </summary>
    /// <param name="id">The unique identifier of the loan to update.</param>
    /// <param name="kolcsonzesWriteDto">The updated loan data.</param>
    /// <returns>An HTTP 204 No Content response when the update succeeds.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="kolcsonzesWriteDto"/> is <c>null</c>.</exception>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when no loan exists with the specified <paramref name="id"/>.</exception>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> PutAsync(Guid id, [FromBody] KolcsonzesWriteDto kolcsonzesWriteDto)
    {
        await kolcsonzesService.PutAsync(id, kolcsonzesWriteDto);
        return NoContent();
    }

    /// <summary>
    /// Deletes a loan (kölcsönzés) by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the loan to delete.</param>
    /// <returns>An HTTP 204 No Content response when the deletion succeeds.</returns>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when no loan exists with the specified <paramref name="id"/>.</exception>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        await kolcsonzesService.DeleteAsync(id);
        return NoContent();
    }
}