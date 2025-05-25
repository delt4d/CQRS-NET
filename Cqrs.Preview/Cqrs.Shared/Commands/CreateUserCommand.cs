using Cqrs.Core;
using Cqrs.Shared.Models;
using Cqrs.Shared.Services;

namespace Cqrs.Shared.Commands;

public record CreateUserCommand(User User) : ICommand;

public class CreateUserCommandHandler(IUserService userService) : ICommandHandler<CreateUserCommand>
{
    public async Task Handle(CreateUserCommand command)
    {
        await userService.Save(command.User);
    }
}