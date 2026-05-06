using MediatR;

public record LogoutCommand(string Username) : IRequest;