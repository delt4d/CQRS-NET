using Cqrs.Shared.Models;
using Cqrs.Shared.Services;

namespace Cqrs.Shared.Queries;

public record GetUserByNameQuery(string Name) : IQuery<Task<User>>;

public class GetUserByNameQueryHandler(UserService userService) : IQueryHandler<GetUserByNameQuery, Task<User>>
{
    public Task<User> Handle(GetUserByNameQuery command, CancellationToken? cancellationToken)
    {
        return userService.GetByName(command.Name, cancellationToken);
    }
}