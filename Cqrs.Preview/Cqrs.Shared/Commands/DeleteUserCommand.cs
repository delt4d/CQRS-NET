using Cqrs.Shared.Services;

namespace Cqrs.Shared.Commands;

public record DeleteUserCommand(string UserId) : ICommand;

public class DeleteUserCommandHandler(UserService userService) : ICommandHandler<DeleteUserCommand>
{
    public Task Handle(DeleteUserCommand command, CancellationToken? cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.UserId))
            throw new InvalidOperationException("User if must have a value to delete.");
        return userService.Delete(command.UserId, cancellationToken);
    }
}

