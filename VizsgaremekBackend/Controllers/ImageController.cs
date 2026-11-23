using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VizsgaremekBackend.Interfaces;

namespace VizsgaremekBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class ImageController(IFileService fileService) : ControllerBase
{
    private const string ImagePrefix = "types";
    
    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        (Stream result, string contentType) =
            await fileService.GetAsync(ImagePrefix + id, cancellationToken);

        return File(result, contentType);
    }

    [Authorize(Roles = "SysAdmin,DeviceAdmin")]
    [HttpPost]
    public async Task<IActionResult> PostAsync(IFormFile file, CancellationToken cancellationToken)
    {
        await fileService.PostAsync(ImagePrefix + Guid.NewGuid(),
            file.OpenReadStream(),
            file.ContentType,
            cancellationToken);

        return Created();
    }

    [Authorize(Roles = "SysAdmin,DeviceAdmin")]
    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await fileService.DeleteAsync(ImagePrefix + id, cancellationToken);
        return NoContent();
    }
}