using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;

namespace VizsgaremekBackend.Interfaces;

public interface IKolcsonzesService
{
    /// <summary>
    /// Retrieves a loan (kölcsönzés) by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the loan to retrieve.</param>
    /// <returns>The mapped <see cref="KolcsonzesReadDto"/> for the requested loan.</returns>
    /// <exception cref="NotFoundException">Thrown when no loan exists with the specified <paramref name="id"/>.</exception>
    public Task<KolcsonzesReadDto> GetAsync(Guid id);

    /// <summary>
    /// Creates a new loan (kölcsönzés).
    /// </summary>
    /// <param name="dto">The data used to create the loan.</param>
    /// <returns>The mapped <see cref="KolcsonzesReadDto"/> representing the created loan.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is <c>null</c>.</exception>
    public Task<KolcsonzesReadDto> PostAsync(KolcsonzesWriteDto dto);

    /// <summary>
    /// Updates an existing loan (kölcsönzés).
    /// </summary>
    /// <param name="id">The unique identifier of the loan to update.</param>
    /// <param name="dto">The updated loan data.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is <c>null</c>.</exception>
    /// <exception cref="NotFoundException">Thrown when no loan exists with the specified <paramref name="id"/>.</exception>
    public Task PutAsync(Guid id, KolcsonzesWriteDto dto);

    /// <summary>
    /// Deletes a loan (kölcsönzés) by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the loan to delete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    /// <exception cref="NotFoundException">Thrown when no loan exists with the specified <paramref name="id"/>.</exception>
    public Task DeleteAsync(Guid id);
}
