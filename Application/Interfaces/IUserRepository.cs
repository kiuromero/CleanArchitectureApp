public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
    Task UpdateAsync(User user);
}