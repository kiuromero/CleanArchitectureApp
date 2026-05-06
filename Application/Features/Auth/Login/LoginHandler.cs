using Application.Interfaces;
using MediatR;

public class LoginHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;

    public LoginHandler(
        IUserRepository userRepository,
        ITokenService tokenService,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);

        if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        var accessToken = _tokenService.GenerateAccessToken(user.Username, user.Role);
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        await _userRepository.UpdateAsync(user);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            ExpiresIn = 900,
            RefreshToken = refreshToken
        };
    }
}