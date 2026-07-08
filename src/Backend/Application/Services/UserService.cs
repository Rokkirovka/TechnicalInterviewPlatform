using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class UserService(
    IUserRepository userRepository,
    IDeletionLogRepository<User> deletionLogRepository,
    IPasswordHasher passwordHasher,
    IMapper mapper,
    ILogger<UserService> logger) : IUserService
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
            throw new KeyNotFoundException($"Пользователь с id {id} не найден");
        return mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request)
    {
        var user = mapper.Map<User>(request);
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
        user.IsActive = true;
        user.Roles = request.Roles;
        await userRepository.AddAsync(user);
        var result = mapper.Map<UserDto>(user);
        
        logger.LogInformation("пользователь {UserId} был создан", result.Id);
        
        return result;
    }

    public async Task<UserDto> UpdateAsync(UpdateUserRequest request)
    {
        var user = await userRepository.GetWithRolesAsync(request.Id);
        if (user == null) throw new KeyNotFoundException($"Пользователь с id {request.Id} не найден");

        mapper.Map(request, user);

        if (!string.IsNullOrWhiteSpace(request.Password))
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
        user.Roles = request.Roles;

        await userRepository.UpdateAsync(user);
        var result = mapper.Map<UserDto>(user);
        
        logger.LogInformation("данные пользователя {UserId} были обновлены", result.Id);
        
        return result;
    }

    public async Task ArchiveAsync(int id, string? reason, int archivedByUserId)
    {
        var user = await userRepository.GetByIdAsync(id);
        if (user == null) throw new KeyNotFoundException($"Пользователь с id {id} не найден");
        if (id == archivedByUserId)
        {
            logger.LogInformation(
                "Пользователь {archivedByUserId} попытался архивировать самого себя - такое нельзя", 
                archivedByUserId);
            
            throw new BusinessRuleConflictException("Вы не можете архивировать самого себя");
        }
        user.DeletedAt = DateTime.UtcNow;
        await userRepository.UpdateAsync(user);
        await deletionLogRepository.AddAsync(user, archivedByUserId, reason);
        
        logger.LogInformation("пользователь {userId} был архивирован", id);
    }

    public async Task RestoreAsync(int id)
    {
        var user = await userRepository.GetByIdAsync(id);
        if (user == null) throw new KeyNotFoundException($"Пользователь с id {id} не найден");
        user.DeletedAt = null;
        await userRepository.UpdateAsync(user);
        
        logger.LogInformation("пользователь {userId} был восстановлен из архива", id);
    }
}