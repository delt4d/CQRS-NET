using Cqrs.Shared.Models;
using Cqrs.Shared.Services;

namespace Cqrs.Shared.CommandsWithResults;

public record UpdateUserResult(User UpdatedUser, DateTime StartedAt, DateTime FinishedAt);
public record UpdateUserCommandWithResult(User UpdateUser) : ICommand<UpdateUserResult>;

public class UpdateUserCommandHandlerWithResult(IUserService userService) : ICommandHandler<UpdateUserCommandWithResult, UpdateUserResult>
{
    public async Task<UpdateUserResult> Handle(UpdateUserCommandWithResult command, CancellationToken? cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.UpdateUser.Id))
            throw new InvalidOperationException($"Id cannot be null when updating user.");

        var startedAt = DateTime.UtcNow;
        var updated = await userService.Update(command.UpdateUser, cancellationToken);

        return new UpdateUserResult(updated, startedAt, DateTime.UtcNow);
    }
}