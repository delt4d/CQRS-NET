using Cqrs.Shared.Models;
using Cqrs.Shared.Services;

namespace Cqrs.Shared.Commands;

public record UpdateUserCommand(User UpdateUser) : ICommand;

public class UpdateUserCommandHandler(UserService userService) : ICommandHandler<UpdateUserCommand>
{
    public Task Handle(UpdateUserCommand command, CancellationToken? cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.UpdateUser.Id))
            throw new InvalidOperationException($"Id cannot be null when updating user.");
        return userService.Update(command.UpdateUser, cancellationToken);
    }
}