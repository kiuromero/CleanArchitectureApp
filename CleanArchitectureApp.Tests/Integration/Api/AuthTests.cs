using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

public class AuthTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthTests(WebApplicationFactory<Program> factory)
    {
        // Seed before creating the client so the test server has the user in the database
        SeedUser(factory);

        _client = factory.CreateClient();
    }

    private void SeedUser(WebApplicationFactory<Program> factory)
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        context.Database.EnsureCreated();

        // 🔥 Limpia para evitar duplicados raros
        context.Users.RemoveRange(context.Users);
        context.SaveChanges();

        var user = new User
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123"),
            Role = "Admin"
        };

        context.Users.Add(user);
        context.SaveChanges();
    }

    [Fact]
    public async Task Login_Should_Return_Token()
    {
        var request = new
        {
            username = "admin",
            password = "123"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/Auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();

        content!.Data.AccessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_Should_Return_401_When_Invalid()
    {
        var request = new
        {
            username = "admin",
            password = "wrong"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/Auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_Should_Return_New_Token()
    {
        // 1. Login
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/Auth/login", new
        {
            username = "admin",
            password = "123"
        });

        var loginData = await loginResponse.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();
        string refreshToken = loginData.Data.RefreshToken;

        // 2. Refresh
        var refreshResponse = await _client.PostAsJsonAsync("/api/v1/Auth/refresh", new
        {
            refreshToken
        });

        refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Logout_Should_Revoke_RefreshToken()
    {
        // Login
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/Auth/login", new
        {
            username = "admin",
            password = "123"
        });

        var raw = await loginResponse.Content.ReadAsStringAsync();        

        var loginData = await loginResponse.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();

        string refreshToken = loginData?.Data.RefreshToken;

        // Logout (needs Authorization)
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginData?.Data.AccessToken);

        var logoutResponse = await _client.PostAsJsonAsync("/api/v1/Auth/logout", new
        {
            refreshToken
        });

        logoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Intentar refresh (debe fallar)
        var refreshResponse = await _client.PostAsJsonAsync("/api/v1/Auth/refresh", new
        {
            refreshToken
        });

        refreshResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}