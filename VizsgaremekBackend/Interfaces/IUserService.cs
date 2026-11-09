using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;

namespace VizsgaremekBackend.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Searches for users by user name using pagination.
    /// </summary>
    /// <param name="query">The search text matched against user names (case-insensitive). Users with a <c>null</c> user name are excluded.</param>
    /// <param name="limit">The maximum number of results to return.</param>
    /// <param name="offset">The number of results to skip.</param>
    /// <returns>A list of <see cref="UserReadDto"/> items matching the search criteria.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="query"/> is <c>null</c>.</exception>
    /// <exception cref="BadRequestException">Thrown when <paramref name="limit"/> or <paramref name="offset"/> is negative.</exception>
    public Task<List<UserReadDto>> SearchAsync(string query, int limit, int offset);

    /// <summary>
    /// Retrieves a user by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to retrieve.</param>
    /// <returns>The mapped <see cref="UserReadDto"/> for the requested user.</returns>
    /// <exception cref="NotFoundException">Thrown when no user exists with the specified <paramref name="id"/>.</exception>
    public Task<UserReadDto> GetAsync(Guid id);

    /// <summary>
    /// Creates a new user with the provided password.
    /// </summary>
    /// <param name="dto">The data used to create the user. The <c>Password</c> field is required.</param>
    /// <returns>The mapped <see cref="UserReadDto"/> representing the created user.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is <c>null</c>.</exception>
    /// <exception cref="BadRequestException">Thrown when the password is missing or the create operation fails with a non-conflict error.</exception>
    /// <exception cref="ConflictException">Thrown when the operation reports a duplicate user name or email.</exception>
    public Task<UserReadDto> PostAsync(UserWriteDto dto);

    /// <summary>
    /// Updates an existing user's mapped properties.
    /// </summary>
    /// <param name="id">The unique identifier of the user to update.</param>
    /// <param name="dto">The updated user data.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is <c>null</c>.</exception>
    /// <exception cref="NotFoundException">Thrown when no user exists with the specified <paramref name="id"/>.</exception>
    public Task PutAsync(Guid id, UserWriteDto dto);

    /// <summary>
    /// Deletes a user by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    /// <exception cref="NotFoundException">Thrown when no user exists with the specified <paramref name="id"/>.</exception>
    /// <exception cref="BadRequestException">Thrown when the delete operation fails.</exception>
    public Task DeleteAsync(Guid id);

    /// <summary>
    /// Retrieves the loans (kölcsönzések) associated with a user.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>An array of <see cref="KolcsonzesReadDto"/> items representing the user's loans.</returns>
    /// <exception cref="NotFoundException">Thrown when no user exists with the specified <paramref name="id"/>.</exception>
    public Task<KolcsonzesReadDto[]> GetKolcsonzesekAsync(Guid id);

    /// <summary>
    /// Retrieves the orders (rendelések) associated with a user.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>An array of <see cref="RendelesReadDto"/> items representing the user's orders.</returns>
    /// <exception cref="NotFoundException">Thrown when no user exists with the specified <paramref name="id"/>.</exception>
    public Task<RendelesReadDto[]> GetRendelesekAsync(Guid id);

    /// <summary>
    /// Retrieves the item instances (példányok) for which a user is responsible.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>An array of <see cref="PeldanyReadDto"/> items representing the user's responsibilities.</returns>
    /// <exception cref="NotFoundException">Thrown when no user exists with the specified <paramref name="id"/>.</exception>
    public Task<PeldanyReadDto[]> GetFelelossegekAsync(Guid id);
}