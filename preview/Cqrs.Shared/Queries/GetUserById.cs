using Cqrs.Shared.Models;
using Cqrs.Shared.Services;

namespace Cqrs.Shared.Queries;

public record GetUserByIdQuery(string Id) : IQuery<User>;

public class GetUserByIdQueryHandler(IUserService userService) : IQueryHandler<GetUserByIdQuery, User>
{
    public Task<User> Handle(GetUserByIdQuery query, CancellationToken? cancellationToken)
    {
        return userService.GetById(query.Id, cancellationToken);
    }
}