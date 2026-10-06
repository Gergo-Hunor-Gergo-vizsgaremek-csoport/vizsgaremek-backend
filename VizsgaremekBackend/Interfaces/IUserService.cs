using VizsgaremekBackend.Dtos;

namespace VizsgaremekBackend.Interfaces;

public interface IUserService
{
    public Task<List<UserReadDto>> SearchAsync(string query, int limit, int offset);

    public Task<UserReadDto> GetAsync(Guid id);

    public Task PostAsync(UserWriteDto dto);

    public Task PutAsync(Guid id, UserWriteDto dto);

    public Task DeleteAsync(Guid id);
}