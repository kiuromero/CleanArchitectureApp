using MediatR;

public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponseDto>;