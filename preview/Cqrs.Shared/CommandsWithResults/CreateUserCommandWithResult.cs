
using Cqrs.Shared.Models;
using Cqrs.Shared.Services;

namespace Cqrs.Shared.CommandsWithResults;

public record CreateUserResult(DateTime StartedAt, DateTime FinishedAt, User User);
public record CreateUserCommandWithResult(User User) : ICommand<CreateUserResult>;

public class CreateUserCommandWithResultHandler(IUserService userService) : ICommandHandler<CreateUserCommandWithResult, CreateUserResult>
{
    public async Task<CreateUserResult> Handle(CreateUserCommandWithResult command, CancellationToken? cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.User.Id))
            throw new InvalidOperationException($"Id cannot be null when create a user.");

        var startedAt = DateTime.UtcNow;
        var savedUser = await userService.Save(command.User, cancellationToken);

        return new CreateUserResult(startedAt, DateTime.UtcNow, savedUser);
    }
}