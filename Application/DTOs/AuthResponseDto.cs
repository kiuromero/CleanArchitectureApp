using System.Text.Json.Serialization;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = null!;

    public int ExpiresIn { get; set; }

    public string RefreshToken { get; set; } = null!;

}