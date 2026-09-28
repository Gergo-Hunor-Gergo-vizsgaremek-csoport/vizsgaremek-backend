using Microsoft.AspNetCore.Mvc;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Services;

namespace VizsgaremekBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class KolcsonzesController(KolcsonzesService kolcsonzesService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<KolcsonzesReadDto>> GetAsync(Guid id)
    {
        return Ok(await kolcsonzesService.GetAsync(id));
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync([FromBody] KolcsonzesWriteDto kolcsonzesWriteDto)
    {
        await kolcsonzesService.PostAsync(kolcsonzesWriteDto);

        return Created();
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> PutAsync(Guid id, [FromBody] KolcsonzesWriteDto kolcsonzesWriteDto)
    {
        await kolcsonzesService.PutAsync(id, kolcsonzesWriteDto);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        await kolcsonzesService.DeleteAsync(id);
        return NoContent();
    }
}