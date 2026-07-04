namespace Application;

public interface IAuthService
{
    Task<LoginResult> LoginAsync(string login, string password, CancellationToken ct = default);
}