using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Services;

namespace VizsgaremekBackend.Controllers;
[ApiController]
[Route("[controller]")]
public class PeldanyController(PeldanyService peldanyService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PeldanyReadDto>> GetAsync(Guid id)
    {
        return Ok(await peldanyService.GetAsync(id));
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync([FromBody] PeldanyWriteDto peldanyWriteDto)
    {
        await peldanyService.PostAsync(peldanyWriteDto);

        return Created();
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> PutAsync(Guid id, [FromBody] PeldanyWriteDto peldanyWriteDto)
    {
        await peldanyService.PutAsync(id, peldanyWriteDto);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        await peldanyService.DeleteAsync(id);
        return NoContent();
    }
}