using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.Interfaces;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(IUserService userService, IUserRoleService userRoleService, IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Retrieves the currently authenticated user.
    /// </summary>
    /// <returns>An HTTP 200 OK response containing the current user's <see cref="UserReadDto"/>.</returns>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotLoggedInException">Thrown when no authenticated user can be resolved from the current request.</exception>
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserReadDto>> CurrentUserAsync()
    {
        User user = await userRoleService.GetCurrentUserAsync(User);

        UserReadDto dto = mapper.Map<UserReadDto>(user);

        return Ok(dto);
    }

    /// <summary>
    /// Retrieves the loans (kölcsönzések) associated with the currently authenticated user.
    /// </summary>
    /// <returns>An HTTP 200 OK response containing the current user's <see cref="KolcsonzesReadDto"/> items.</returns>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotLoggedInException">Thrown when no authenticated user can be resolved from the current request.</exception>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when the resolved current user cannot be found.</exception>
    [Authorize]
    [HttpGet("me/kolcsonzesek")]
    public async Task<ActionResult<KolcsonzesReadDto[]>> CurrentUserKolcsonzesekAsync()
    {
        User user = await userRoleService.GetCurrentUserAsync(User);
        return Ok(await userService.GetKolcsonzesekAsync(user.Id));
    }

    /// <summary>
    /// Retrieves the orders (rendelések) associated with the currently authenticated user.
    /// </summary>
    /// <returns>An HTTP 200 OK response containing the current user's <see cref="RendelesReadDto"/> items.</returns>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotLoggedInException">Thrown when no authenticated user can be resolved from the current request.</exception>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when the resolved current user cannot be found.</exception>
    [Authorize]
    [HttpGet("me/rendelesek")]
    public async Task<ActionResult<KolcsonzesReadDto[]>> CurrentUserRendelesekAsync()
    {
        User user = await userRoleService.GetCurrentUserAsync(User);
        return Ok(await userService.GetRendelesekAsync(user.Id));
    }

    /// <summary>
    /// Retrieves the item instances (példányok) for which the currently authenticated user is responsible.
    /// </summary>
    /// <returns>An HTTP 200 OK response containing the current user's <see cref="PeldanyReadDto"/> items.</returns>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotLoggedInException">Thrown when no authenticated user can be resolved from the current request.</exception>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when the resolved current user cannot be found.</exception>
    [Authorize]
    [HttpGet("me/felelossegek")]
    public async Task<ActionResult<KolcsonzesReadDto[]>> CurrentUserFelelossegekAsync()
    {
        User user = await userRoleService.GetCurrentUserAsync(User);
        return Ok(await userService.GetFelelossegekAsync(user.Id));
    }

    /// <summary>
    /// Searches for users by user name using optional pagination.
    /// </summary>
    /// <param name="q">The optional search query matched against user names. Defaults to an empty string when <c>null</c>.</param>
    /// <param name="limit">The maximum number of results to return. Defaults to 50 and may not exceed 100.</param>
    /// <param name="offset">The number of results to skip for pagination. Defaults to 0.</param>
    /// <returns>An HTTP 200 OK response containing the matching <see cref="UserReadDto"/> items.</returns>
    /// <exception cref="BadRequestException">Thrown when <paramref name="limit"/> is greater than 100, or when <paramref name="limit"/> or <paramref name="offset"/> is negative.</exception>
    [HttpGet]
    public async Task<ActionResult<List<UserReadDto>>> SearchAsync(string? q, int? limit, int? offset)
    {
        const int maxResults = 100;
        const int defaultLimit = 50;

        if (limit is > maxResults) throw new BadRequestException($"Limit may not be more than {maxResults}.");
        if (limit is < 0) throw new BadRequestException("Limit must be greater than or equal to 0.");
        if (offset is < 0) throw new BadRequestException("Offset must be greater than or equal to 0.");

        limit ??= defaultLimit;
        offset ??= 0;
        q ??= string.Empty;

        List<UserReadDto> result = await userService.SearchAsync(q, limit.Value, offset.Value);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a user by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to retrieve.</param>
    /// <returns>An HTTP 200 OK response containing the requested <see cref="UserReadDto"/>.</returns>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when no user exists with the specified <paramref name="id"/>.</exception>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserReadDto>> GetAsync(Guid id)
    {
        return Ok(await userService.GetAsync(id));
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="userWriteDto">The data used to create the user, including the required password.</param>
    /// <returns>An HTTP 201 Created response containing the created <see cref="UserReadDto"/> and its location.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="userWriteDto"/> is <c>null</c>.</exception>
    /// <exception cref="BadRequestException">Thrown when the password is missing or the identity operation fails with a non-conflict error.</exception>
    /// <exception cref="ConflictException">Thrown when a user with the same user name or email already exists.</exception>
    [HttpPost]
    public async Task<ActionResult> PostAsync([FromBody] UserWriteDto userWriteDto)
    {
        UserReadDto res = await userService.PostAsync(userWriteDto);

        return Created($"/User/{res.Id}", res);
    }

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="id">The unique identifier of the user to update.</param>
    /// <param name="userWriteDto">The updated user data.</param>
    /// <returns>An HTTP 204 No Content response when the update succeeds.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="userWriteDto"/> is <c>null</c>.</exception>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when no user exists with the specified <paramref name="id"/>.</exception>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> PutAsync(Guid id, [FromBody] UserWriteDto userWriteDto)
    {
        await userService.PutAsync(id, userWriteDto);
        return NoContent();
    }

    /// <summary>
    /// Deletes a user by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <returns>An HTTP 204 No Content response when the deletion succeeds.</returns>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when no user exists with the specified <paramref name="id"/>.</exception>
    /// <exception cref="BadRequestException">Thrown when the identity delete operation fails.</exception>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        await userService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Retrieves the loans (kölcsönzések) associated with a user.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>An HTTP 200 OK response containing the user's <see cref="KolcsonzesReadDto"/> items.</returns>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when no user exists with the specified <paramref name="id"/>.</exception>
    [HttpGet("{id:guid}/kolcsonzesek")]
    public async Task<ActionResult<KolcsonzesReadDto[]>> GetKolcsonzesekAsync(Guid id)
    {
        return Ok(await userService.GetKolcsonzesekAsync(id));
    }

    /// <summary>
    /// Retrieves the orders (rendelések) associated with a user.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>An HTTP 200 OK response containing the user's <see cref="RendelesReadDto"/> items.</returns>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when no user exists with the specified <paramref name="id"/>.</exception>
    [HttpGet("{id:guid}/rendelesek")]
    public async Task<ActionResult<RendelesReadDto[]>> GetRendelesekAsync(Guid id)
    {
        return Ok(await userService.GetRendelesekAsync(id));
    }

    /// <summary>
    /// Retrieves the item instances (példányok) for which a user is responsible.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>An HTTP 200 OK response containing the user's <see cref="PeldanyReadDto"/> items.</returns>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when no user exists with the specified <paramref name="id"/>.</exception>
    [HttpGet("{id:guid}/felelossegek")]
    public async Task<ActionResult<PeldanyReadDto[]>> GetFelelossegekAsync(Guid id)
    {
        return Ok(await userService.GetFelelossegekAsync(id));

    }

    /// <summary>
    /// Adds or removes the <c>SysAdmin</c> role for a user.
    /// </summary>
    /// <param name="id">The unique identifier of the target user.</param>
    /// <param name="set"><c>true</c> to add the role; <c>false</c> to remove it. Defaults to <c>true</c>.</param>
    /// <returns>An HTTP 200 OK response when the role change succeeds.</returns>
    /// <exception cref="BadRequestException">Thrown when the role operation fails with a non-conflict error.</exception>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when no user exists with the specified <paramref name="id"/>.</exception>
    /// <exception cref="ConflictException">Thrown when the user already has the role or does not have it when removal is requested.</exception>
    [HttpPut("{id:guid}/SysAdmin")]
    public async Task<ActionResult> SetSysAdminRoleAsync(Guid id, bool set = true)
    {
        await userRoleService.SetSysAdminRoleAsync(id, set);

        return Ok();
    }

    /// <summary>
    /// Adds or removes the <c>UserAdmin</c> role for a user.
    /// </summary>
    /// <param name="id">The unique identifier of the target user.</param>
    /// <param name="set"><c>true</c> to add the role; <c>false</c> to remove it. Defaults to <c>true</c>.</param>
    /// <returns>An HTTP 200 OK response when the role change succeeds.</returns>
    /// <exception cref="BadRequestException">Thrown when the role operation fails with a non-conflict error.</exception>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when no user exists with the specified <paramref name="id"/>.</exception>
    /// <exception cref="ConflictException">Thrown when the user already has the role or does not have it when removal is requested.</exception>
    [HttpPut("{id:guid}/UserAdmin")]
    public async Task<ActionResult> SetUserAdminRoleAsync(Guid id, bool set = true)
    {
        await userRoleService.SetUserAdminRoleAsync(id, set);

        return Ok();
    }

    /// <summary>
    /// Adds or removes the <c>DeviceAdmin</c> role for a user.
    /// </summary>
    /// <param name="id">The unique identifier of the target user.</param>
    /// <param name="set"><c>true</c> to add the role; <c>false</c> to remove it. Defaults to <c>true</c>.</param>
    /// <returns>An HTTP 200 OK response when the role change succeeds.</returns>
    /// <exception cref="BadRequestException">Thrown when the role operation fails with a non-conflict error.</exception>
    /// <exception cref="VizsgaremekBackend.Exceptions.NotFoundException">Thrown when no user exists with the specified <paramref name="id"/>.</exception>
    /// <exception cref="ConflictException">Thrown when the user already has the role or does not have it when removal is requested.</exception>
    [HttpPut("{id:guid}/DeviceAdmin")]
    public async Task<ActionResult> SetDeviceAdminRoleAsync(Guid id, bool set = true)
    {
        await userRoleService.SetDeviceAdminRoleAsync(id, set);

        return Ok();
    }
}