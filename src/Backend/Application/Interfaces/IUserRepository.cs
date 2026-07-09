using Domain.Entities;

namespace Application.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<IReadOnlyList<User>> SearchAsync(string? search, bool showArchived);
    Task<User?> GetByLoginAsync(string login);
    Task<User?> GetWithRolesAsync(int id);
}