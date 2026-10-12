using Microsoft.AspNetCore.Mvc;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Services;

namespace VizsgaremekBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class RendelesController(RendelesService rendelesService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RendelesReadDto>> GetAsync(Guid id)
    {
        return Ok(await rendelesService.GetAsync(id));
    }
    
    
    [HttpPost]
    public async Task<ActionResult> PostAsync(RendelesWriteDto rendelesWriteDto)
    {
        var createdRendeles = await rendelesService.PostAsync(rendelesWriteDto);
        return Created($"/api/users/{createdRendeles.Id}", createdRendeles);
    }    
    
    
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> PutAsync(Guid id, [FromBody] RendelesWriteDto rendelesWriteDto)
    {
        await rendelesService.PutAsync(id, rendelesWriteDto);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        await rendelesService.DeleteAsync(id);
        return NoContent();
    }

    //kölcsönzéseket csinál a kiválasztott példányokból a rendeléshez
    [HttpPost("{id:guid}/assign")]
    public async Task<ActionResult<KolcsonzesReadDto[]>> AssignPeldanyAsync(KolcsonzesWriteDto[] kolcsonzesWriteDtos)
    {
        if (kolcsonzesWriteDtos.Length == 0)
        {
            return BadRequest("Array size should be greater than 0");
        }

        var createdKolcsonzesek = await rendelesService.AssignPeldanysAsync(kolcsonzesWriteDtos);
        return Created($"/api/kolcsonzes/{createdKolcsonzesek[0].Id}",createdKolcsonzesek);
    }
}