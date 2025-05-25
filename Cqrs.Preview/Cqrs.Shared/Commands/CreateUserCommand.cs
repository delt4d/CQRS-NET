using Cqrs.Shared.Models;
using Cqrs.Shared.Services;

namespace Cqrs.Shared.Commands;

public record CreateUserCommand(User User) : ICommand;

public class CreateUserCommandHandler(IUserService userService) : ICommandHandler<CreateUserCommand>
{
    public async Task Handle(CreateUserCommand command, CancellationToken? cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.User.Id))
            throw new InvalidOperationException($"Id cannot be null when create a user.");
        await userService.Save(command.User, cancellationToken);
    }
}