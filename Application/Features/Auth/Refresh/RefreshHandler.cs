using MediatR;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public RefreshTokenHandler(
        IUserRepository userRepository,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken);

        if (user == null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        var token = user.RefreshTokens.First(x => x.Token == request.RefreshToken);

        if (!token.IsActive)
            throw new UnauthorizedAccessException("Token expired or revoked");

        token.IsRevoked = true;

        var newRefreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        var newAccessToken = _tokenService.GenerateAccessToken(user.Username, user.Role);

        await _userRepository.UpdateAsync(user);

        return new AuthResponseDto
        {
            AccessToken = newAccessToken,
            ExpiresIn = 900,
            RefreshToken = newRefreshToken
        };
    }
}