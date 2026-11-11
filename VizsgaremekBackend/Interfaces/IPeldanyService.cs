using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;

namespace VizsgaremekBackend.Interfaces;

public interface IPeldanyService
{
    /// <summary>
    /// Retrieves an item instance (példány) by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the instance to retrieve.</param>
    /// <returns>The mapped <see cref="PeldanyReadDto"/> for the requested instance.</returns>
    /// <exception cref="NotFoundException">Thrown when no instance exists with the specified <paramref name="id"/>.</exception>
    public Task<PeldanyReadDto> GetAsync(Guid id);

    /// <summary>
    /// Creates a new item instance (példány).
    /// </summary>
    /// <param name="dto">The data used to create the instance.</param>
    /// <returns>The mapped <see cref="PeldanyReadDto"/> representing the created instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is <c>null</c>.</exception>
    public Task<PeldanyReadDto> PostAsync(PeldanyWriteDto dto);

    /// <summary>
    /// Updates an existing item instance (példány).
    /// </summary>
    /// <param name="id">The unique identifier of the instance to update.</param>
    /// <param name="dto">The updated instance data.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is <c>null</c>.</exception>
    /// <exception cref="NotFoundException">Thrown when no instance exists with the specified <paramref name="id"/>.</exception>
    public Task PutAsync(Guid id, PeldanyWriteDto dto);

    /// <summary>
    /// Deletes an item instance (példány) by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the instance to delete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    /// <exception cref="NotFoundException">Thrown when no instance exists with the specified <paramref name="id"/>.</exception>
    public Task DeleteAsync(Guid id);
}
