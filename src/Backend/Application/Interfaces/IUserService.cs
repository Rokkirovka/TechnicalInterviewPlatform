using Application.Dtos;

namespace Application.Interfaces;

public interface IUserService : IBaseService<UserDto, CreateUserRequest, UpdateUserRequest>
{
    Task ChangePasswordAsync(ChangeUserPasswordRequest request);
}