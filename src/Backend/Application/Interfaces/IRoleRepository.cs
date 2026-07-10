using Domain.Entities;

namespace Application.Interfaces;

public interface IRoleRepository
{
    Task<IReadOnlyList<Role>> GetAllAsync();
}