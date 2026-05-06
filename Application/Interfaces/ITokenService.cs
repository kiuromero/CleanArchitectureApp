public interface ITokenService
{
    string GenerateAccessToken(string username, string role);
    int GetAccessTokenExpirationSeconds();
    string GenerateRefreshToken();
}