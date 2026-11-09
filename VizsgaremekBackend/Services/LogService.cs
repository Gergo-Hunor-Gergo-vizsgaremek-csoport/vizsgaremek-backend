using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VizsgaremekBackend.Data;
using VizsgaremekBackend.Dtos;
using VizsgaremekBackend.Exceptions;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.Services;

public class LogService(VizsgaremekContext vizsgaremekContext, IMapper mapper)
{
    /// <summary>
    /// Searches for log entries by type using pagination.
    /// </summary>
    /// <param name="query">The search text matched against log entry types (case-insensitive).</param>
    /// <param name="limit">The maximum number of results to return.</param>
    /// <param name="offset">The number of results to skip.</param>
    /// <returns>A list of <see cref="LogReadDto"/> items matching the search criteria.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="query"/> is <c>null</c>.</exception>
    /// <exception cref="BadRequestException">Thrown when <paramref name="limit"/> or <paramref name="offset"/> is negative.</exception>
    public async Task<List<LogReadDto>> SearchAsync(string query, int limit, int offset)
    {
        ArgumentNullException.ThrowIfNull(query);
        if (limit < 0) throw new BadRequestException("Limit must be greater than or equal to 0.");
        if (offset < 0) throw new BadRequestException("Offset must be greater than or equal to 0.");

        return (await vizsgaremekContext.Logs
                .Where(x => x.Type.ToLower().Contains(query.ToLower()))
                .Skip(offset)
                .Take(limit)
                .ToArrayAsync())
            .Select(mapper.Map<LogReadDto>)
            .ToList();
    }

    /// <summary>
    /// Retrieves a log entry by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the log entry to retrieve.</param>
    /// <returns>The mapped <see cref="LogReadDto"/> for the requested log entry.</returns>
    /// <exception cref="NotFoundException">Thrown when no log entry exists with the specified <paramref name="id"/>.</exception>
    public async Task<LogReadDto> GetAsync(Guid id)
    {
        Log result = await vizsgaremekContext.Logs.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Log), id);

        return mapper.Map<LogReadDto>(result);
    }

    /// <summary>
    /// Creates a new log entry.
    /// </summary>
    /// <param name="dto">The data used to create the log entry.</param>
    /// <returns>The mapped <see cref="LogReadDto"/> representing the created log entry.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is <c>null</c>.</exception>
    public async Task<LogReadDto> PostAsync(LogWriteDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Log newLog = mapper.Map<Log>(dto);

        await vizsgaremekContext.Logs.AddAsync(newLog);
        await vizsgaremekContext.SaveChangesAsync();

        return mapper.Map<LogReadDto>(newLog);
    }

    /// <summary>
    /// Updates an existing log entry.
    /// </summary>
    /// <param name="id">The unique identifier of the log entry to update.</param>
    /// <param name="dto">The updated log entry data.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is <c>null</c>.</exception>
    /// <exception cref="NotFoundException">Thrown when no log entry exists with the specified <paramref name="id"/>.</exception>
    public async Task PutAsync(Guid id, LogWriteDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Log oldLog = await vizsgaremekContext.Logs.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Log), id);

        mapper.Map(dto, oldLog);
        await vizsgaremekContext.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a log entry by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the log entry to delete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    /// <exception cref="NotFoundException">Thrown when no log entry exists with the specified <paramref name="id"/>.</exception>
    public async Task DeleteAsync(Guid id)
    {
        Log log = await vizsgaremekContext.Logs.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException(nameof(Log), id);

        vizsgaremekContext.Logs.Remove(log);
        await vizsgaremekContext.SaveChangesAsync();
    }
}