using MediatR;

public class LogoutHandler : IRequestHandler<LogoutCommand>
{
    private readonly IUserRepository _userRepository;

    public LogoutHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);

        if (user == null)
            return;

        foreach (var token in user.RefreshTokens)
        {
            token.IsRevoked = true;
        }

        await _userRepository.UpdateAsync(user);
    }
}