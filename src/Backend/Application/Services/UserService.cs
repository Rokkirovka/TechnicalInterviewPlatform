using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services;

public class UserService(
    IRepository<User> repository,
    IDeletionLogRepository<User> deletionLogRepository,
    IRepository<Role> roleRepository,
    IMapper mapper)
    : BaseService<User, UserDto, CreateUserRequest, UpdateUserRequest>(
        repository,
        deletionLogRepository,
        mapper),
      IUserService
{
    public override async Task<UserDto> CreateAsync(CreateUserRequest request)
    {
        var existingUsers = await Repository.AllAliveAsync();
        if (existingUsers.Any(u => u.Login == request.Login))
            throw new Exception($"Пользователь с логином '{request.Login}' уже существует");

        var user = Mapper.Map<User>(request);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        if (request.Roles.Any())
        {
            var allRoles = await roleRepository.AllAliveAsync();
            var existingRoles = allRoles.Where(r => request.Roles.Contains(r.Name)).ToList();
            user.Roles = existingRoles;
        }

        var result = await Repository.AddAsync(user);
        return Mapper.Map<UserDto>(result);
    }

    public override async Task<UserDto> UpdateAsync(UpdateUserRequest request)
    {
        var user = await Repository.GetByIdAsync(request.Id);
        if (user == null)
            throw new Exception($"Пользователь с id {request.Id} не найден");

        Mapper.Map(request, user);

        user.Roles.Clear();
        if (request.Roles.Any())
        {
            var allRoles = await roleRepository.AllAliveAsync();
            var existingRoles = allRoles.Where(r => request.Roles.Contains(r.Name)).ToList();
            user.Roles = existingRoles;
        }

        await Repository.UpdateAsync(user);
        return Mapper.Map<UserDto>(user);
    }

    public async Task ChangePasswordAsync(ChangeUserPasswordRequest request)
    {
        var user = await Repository.GetByIdAsync(request.Id);
        if (user == null)
            throw new Exception($"Пользователь с id {request.Id} не найден");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await Repository.UpdateAsync(user);
    }
}