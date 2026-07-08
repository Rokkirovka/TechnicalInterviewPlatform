using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services;

public class UserService(
    IUserRepository userRepository,
    IDeletionLogRepository<User> deletionLogRepository,
    IMapper mapper) : IUserService
{
    public async Task<IReadOnlyList<UserDto>> SearchAsync(string? search, bool showArchived)
    {
        var users = await userRepository.SearchAsync(search, showArchived);
        return mapper.Map<IReadOnlyList<UserDto>>(users);
    }

    public async Task<UserDto> GetByIdAsync(int id)
    {
        var user = await userRepository.GetWithRolesAsync(id);
        if (user == null)
            throw new Exception($"Пользователь с id {id} не найден");
        return mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request)
    {
        var user = mapper.Map<User>(request);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        user.IsActive = true;
        user.Roles = request.Roles;
        await userRepository.AddAsync(user);
        return mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> UpdateAsync(UpdateUserRequest request)
    {
        var user = await userRepository.GetWithRolesAsync(request.Id);
        if (user == null) throw new Exception($"Пользователь с id {request.Id} не найден");

        mapper.Map(request, user);

        if (!string.IsNullOrWhiteSpace(request.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        user.Roles = request.Roles;

        await userRepository.UpdateAsync(user);
        return mapper.Map<UserDto>(user);
    }

    public async Task ArchiveAsync(int id, string? reason, int archivedByUserId)
    {
        var user = await userRepository.GetByIdAsync(id);
        if (user == null) throw new Exception($"Пользователь с id {id} не найден");
        if (id == archivedByUserId) throw new Exception("Вы не можете архивировать самого себя");
        user.DeletedAt = DateTime.UtcNow;
        await userRepository.UpdateAsync(user);
        await deletionLogRepository.AddAsync(user, archivedByUserId, reason);
    }

    public async Task RestoreAsync(int id)
    {
        var user = await userRepository.GetByIdAsync(id);
        if (user == null) throw new Exception($"Пользователь с id {id} не найден");
        user.DeletedAt = null;
        await userRepository.UpdateAsync(user);
    }
}