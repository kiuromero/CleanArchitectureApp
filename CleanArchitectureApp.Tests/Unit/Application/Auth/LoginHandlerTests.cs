using FluentAssertions;
using Moq;
using Xunit;

public class LoginHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();

    [Fact]
    public async Task Should_Return_Tokens_When_Credentials_Are_Valid()
    {
        var user = new User
        {
            Username = "admin",
            PasswordHash = "hashed",
            Role = "Admin"
        };

        _userRepo.Setup(x => x.GetByUsernameAsync("admin"))
            .ReturnsAsync(user);

        _passwordHasher.Setup(x => x.Verify("123", "hashed"))
            .Returns(true);

        _tokenService.Setup(x => x.GenerateAccessToken("admin", "Admin"))
            .Returns("access_token");

        _tokenService.Setup(x => x.GenerateRefreshToken())
            .Returns("refresh_token");

        _tokenService.Setup(x => x.GetAccessTokenExpirationSeconds())
            .Returns(900);

        var handler = new LoginHandler(
            _userRepo.Object,
            _tokenService.Object,
            _passwordHasher.Object
        );

        var result = await handler.Handle(
            new LoginCommand("admin", "123"),
            CancellationToken.None
        );

        result.AccessToken.Should().Be("access_token");
        result.RefreshToken.Should().Be("refresh_token");
        result.ExpiresIn.Should().Be(900);
    }

    [Fact]
    public async Task Should_Throw_When_Password_Is_Invalid()
    {
        // Arrange
        var user = new User
        {
            Username = "admin",
            PasswordHash = "hashed"
        };

        _userRepo
            .Setup(x => x.GetByUsernameAsync("admin"))
            .ReturnsAsync(user);

        _passwordHasher
            .Setup(x => x.Verify("wrong", "hashed"))
            .Returns(false);

        var handler = new LoginHandler(
            _userRepo.Object,
            _tokenService.Object,
            _passwordHasher.Object
        );

        var command = new LoginCommand("admin", "wrong");

        // Act
        Func<Task> act = async () =>
            await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid credentials");
    }
}