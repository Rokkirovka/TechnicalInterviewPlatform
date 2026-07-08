using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services;

public class UserService(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IDeletionLogRepository<User> deletionLogRepository,
    IPasswordHasher passwordHasher,
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
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
        user.IsActive = true;
        if (request.Roles.Any())
        {
            var allRoles = await roleRepository.GetAllAsync();
            var existingRoles = allRoles.Where(r => request.Roles.Contains(r.Name)).ToList();
            user.Roles = existingRoles;
        }
        await userRepository.AddAsync(user);
        return mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> UpdateAsync(int id, UpdateUserRequest request)
    {
        var user = await userRepository.GetWithRolesAsync(id);
        if (user == null) throw new Exception($"Пользователь с id {id} не найден");

        mapper.Map(request, user);

        if (!string.IsNullOrWhiteSpace(request.Password))
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
        if (request.Roles.Count != 0)
        {
            var allRoles = await roleRepository.GetAllAsync();
            var existingRoles = allRoles.Where(r => request.Roles.Contains(r.Name)).ToList();
            user.Roles = existingRoles;
        }

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