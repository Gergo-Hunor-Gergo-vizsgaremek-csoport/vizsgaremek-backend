using Microsoft.AspNetCore.Mvc;

namespace VizsgaremekBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class ImageController : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /*
    [HttpGet("{id:guid}/metadata")]
    public async Task<IActionResult> GetMetadataAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    */

    [HttpPost]
    public async Task<IActionResult> PostAsync(IFormFile file, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}