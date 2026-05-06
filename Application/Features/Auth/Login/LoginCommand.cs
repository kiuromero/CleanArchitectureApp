using MediatR;

public record LoginCommand(string Username, string Password) : IRequest<AuthResponseDto>;