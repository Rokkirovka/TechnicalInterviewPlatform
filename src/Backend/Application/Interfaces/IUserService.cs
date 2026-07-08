using Application.Dtos;

namespace Application.Interfaces;

public interface IUserService
{
    Task<IReadOnlyList<UserDto>> SearchAsync(string? search, bool showArchived);
    Task<UserDto> GetByIdAsync(int id);
    Task<UserDto> CreateAsync(CreateUserRequest request);
    Task<UserDto> UpdateAsync(int id, UpdateUserRequest request);
    Task ArchiveAsync(int id, string? reason, int archivedByUserId);
    Task RestoreAsync(int id);
}