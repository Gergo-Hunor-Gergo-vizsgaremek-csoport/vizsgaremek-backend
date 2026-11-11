using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;

namespace VizsgaremekBackend.Interfaces;

public interface IRendelesService
{
    /// <summary>
    /// Retrieves an order (rendelés) by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the order to retrieve.</param>
    /// <returns>The mapped <see cref="RendelesReadDto"/> for the requested order.</returns>
    /// <exception cref="NotFoundException">Thrown when no order exists with the specified <paramref name="id"/>.</exception>
    public Task<RendelesReadDto> GetAsync(Guid id);

    /// <summary>
    /// Creates a new order (rendelés).
    /// </summary>
    /// <param name="dto">The data used to create the order.</param>
    /// <returns>The mapped <see cref="RendelesReadDto"/> representing the created order.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is <c>null</c>.</exception>
    public Task<RendelesReadDto> PostAsync(RendelesWriteDto dto);

    /// <summary>
    /// Updates an existing order (rendelés).
    /// </summary>
    /// <param name="id">The unique identifier of the order to update.</param>
    /// <param name="dto">The updated order data.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is <c>null</c>.</exception>
    /// <exception cref="NotFoundException">Thrown when no order exists with the specified <paramref name="id"/>.</exception>
    public Task PutAsync(Guid id, RendelesWriteDto dto);

    /// <summary>
    /// Deletes an order (rendelés) by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the order to delete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    /// <exception cref="NotFoundException">Thrown when no order exists with the specified <paramref name="id"/>.</exception>
    public Task DeleteAsync(Guid id);

    /// <summary>
    /// Assigns item instances (példányok) to an order by creating loans (kölcsönzések) from the provided assignment data.
    /// </summary>
    /// <param name="rendelesId">The unique identifier of the target order.</param>
    /// <param name="kolcsonzesWriteDtos">The loan data for each assignment to create. Each entry is mapped to a loan and linked to its referenced instance.</param>
    /// <returns>An array of <see cref="KolcsonzesReadDto"/> items representing the created loans, in the same order as the input.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="kolcsonzesWriteDtos"/> is <c>null</c>.</exception>
    /// <exception cref="BadRequestException">Thrown when <paramref name="kolcsonzesWriteDtos"/> is empty or contains a <c>null</c> element.</exception>
    /// <exception cref="NotFoundException">Thrown when the order or a referenced instance (példány) does not exist.</exception>
    /// <exception cref="ConflictException">Thrown when a referenced instance already has an active loan.</exception>
    /// <remarks>
    /// For each assignment, the method verifies the order exists, resolves the referenced instance, ensures the instance has no active loan,
    /// sets the instance's active loan reference, and adds the new loan. All changes are persisted in a single <c>SaveChangesAsync</c> call.
    /// </remarks>
    public Task<KolcsonzesReadDto[]> AssignPeldanysAsync(Guid rendelesId, KolcsonzesWriteDto[] kolcsonzesWriteDtos);
}
