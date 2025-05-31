using Cqrs.Shared.Models;
using Cqrs.Shared.Services;

namespace Cqrs.Shared.Queries;

public record GetUserByNameQuery(string Name) : IQuery<User>;

public class GetUserByNameQueryHandler(IUserService userService) : IQueryHandler<GetUserByNameQuery, User>
{
    public Task<User> Handle(GetUserByNameQuery command, CancellationToken? cancellationToken)
    {
        return userService.GetByName(command.Name, cancellationToken);
    }
}