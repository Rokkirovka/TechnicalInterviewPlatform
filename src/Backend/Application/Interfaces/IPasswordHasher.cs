using Domain.Entities;

namespace Application.Interfaces;

public interface IPasswordHasher
{
    public string HashPassword(User user, string password);
    public bool IsPasswordVerified(User user, string password);
}