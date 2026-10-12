using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Interfaces;
using VizsgaremekBackend.Models;
using VizsgaremekBackend.Services;

namespace VizsgaremekBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(IUserService userService, UserRoleService userRoleService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<UserReadDto>>> SearchAsync(string? q, int? limit, int? offset)
    {
        const int maxResults = 100;
        const int defaultLimit = 50;
        
        if (limit > maxResults) return BadRequest($"Limit may not be more than {maxResults}");
        limit ??= defaultLimit;

        offset ??= 0;
        q ??= "";
        
        List<UserReadDto> result = await userService.SearchAsync(q, (int)limit, (int)offset);
        return Ok(result);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserReadDto>> GetAsync(Guid id)
    {
        return Ok(await userService.GetAsync(id));
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync([FromBody] UserWriteDto userWriteDto)
    {
        await userService.PostAsync(userWriteDto);

        return Created();
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> PutAsync(Guid id, [FromBody] UserWriteDto userWriteDto)
    {
        await userService.PutAsync(id, userWriteDto);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        await userService.DeleteAsync(id);
        return NoContent();
    }
    
    
    [HttpGet("{id:guid}/kolcsonzesek")]
    public async Task<ActionResult<KolcsonzesReadDto[]>> GetKolcsonzesekAsync(Guid id)
    {
        return Ok(await userService.GetKolcsonzesekAsync(id));
    }
    
    [HttpGet("{id:guid}/rendelesek")]
    public async Task<ActionResult<RendelesReadDto[]>> GetRendelesekAsync(Guid id)
    {
        return Ok(await userService.GetRendelesekAsync(id));
    } 
    
    [HttpGet("{id:guid}/felelossegek")]
    public async Task<ActionResult<PeldanyReadDto[]>> GetFelelossegekAsync(Guid id)
    {
        return Ok(await userService.GetFelelossegekAsync(id));

    }

    [HttpPut("{id:guid}/SysAdmin")]
    public async Task<ActionResult> SetSysAdminRoleAsync(Guid id, bool set = true)
    {
        await userRoleService.SetSysAdminRoleAsync(id, set);
        
        return Ok();
    }

    [HttpPut("{id:guid}/UserAdmin")]
    public async Task<ActionResult> SetUserAdminRoleAsync(Guid id, bool set = true)
    {
        await userRoleService.SetUserAdminRoleAsync(id, set);
        
        return Ok();
    }

    [HttpPut("{id:guid}/DeviceAdmin")]
    public async Task<ActionResult> SetDeviceAdminRoleAsync(Guid id, bool set = true)
    {
        await userRoleService.SetDeviceAdminRoleAsync(id, set);
        
        return Ok();
    }
}