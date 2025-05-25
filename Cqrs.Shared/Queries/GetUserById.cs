using Cqrs.Core;
using Cqrs.Shared.Models;
using Cqrs.Shared.Services;

namespace Cqrs.Shared.Queries;

public record GetUserByIdQuery(string Id) : IQuery<Task<User>>;

public class GetUserByIdQueryHandler(IUserService userService) : IQueryHandler<GetUserByIdQuery, Task<User>>
{
    public Task<User> Handle(GetUserByIdQuery query)
    {
        return userService.GetById(query.Id);
    }
}