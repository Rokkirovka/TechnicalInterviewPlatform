using Domain.Entities;

namespace Domain;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<User?> GetByLoginAsync(string login, CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
    Task UpdateRolesAsync(int id, List<Role> roles, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}