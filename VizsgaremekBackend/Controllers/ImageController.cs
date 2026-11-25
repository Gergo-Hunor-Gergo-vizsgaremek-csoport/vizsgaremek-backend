using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.Interfaces;

namespace VizsgaremekBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class ImageController(IFileService fileService) : ControllerBase
{
    private const string ImagePrefix = "types/";
    
    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        (Stream result, string contentType) =
            await fileService.GetAsync(ImagePrefix + id, cancellationToken);

        return File(result, contentType);
    }

    private static class UploadLimits
    {
        public const long MaxImageBytes = 5 * 1024 * 1024;           // 5,242,880 (5 MiB)
        public const long MaxRequestBytes = MaxImageBytes + 64 * 1024; // + headroom for multipart overhead
    }

    [Authorize(Roles = "SysAdmin,DeviceAdmin")]
    [HttpPost]
    [RequestSizeLimit(UploadLimits.MaxRequestBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = UploadLimits.MaxImageBytes)]
    public async Task<IActionResult> PostAsync(IFormFile file, CancellationToken cancellationToken)
    {
        Guid id = Guid.NewGuid();

        if (file.ContentType is not ("image/jpeg" or "image/png" or "image/webp")) 
            return Problem(
                title: "Unsupported image type.",
                detail: "Allowed types: image/jpeg, image/png, image/webp.",
                statusCode: StatusCodes.Status415UnsupportedMediaType);

        await using Stream stream = file.OpenReadStream();

        using SKManagedStream skStream = new(stream, false); // don't dispose caller's stream
        using SKCodec? codec = SKCodec.Create(skStream);              // null if not a recognized image

        string? realType = codec?.EncodedFormat switch
        {
            SKEncodedImageFormat.Jpeg => "image/jpeg",
            SKEncodedImageFormat.Png => "image/png",
            SKEncodedImageFormat.Webp => "image/webp",
            _ => null,
        };

        if (file.ContentType != realType) throw new BadRequestException("Image doesent match provided image type.");
        
        stream.Position = 0;
        
        await fileService.PostAsync(ImagePrefix + id,
            stream,
            file.ContentType,
            cancellationToken);

        return Created(id.ToString(), null);
    }

    [Authorize(Roles = "SysAdmin,DeviceAdmin")]
    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await fileService.DeleteAsync(ImagePrefix + id, cancellationToken);
        return NoContent();
    }
}