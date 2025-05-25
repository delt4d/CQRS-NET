using Cqrs.Core;
using Cqrs.Core.Providers;
using Cqrs.Core.RegisterResolver;
using Cqrs.Shared.Commands;
using Cqrs.Shared.Models;
using Cqrs.Shared.Queries;
using Cqrs.Shared.Services;

namespace Cqrs.Console;

class Program
{
    static async Task Main(string[] args)
    {
        var userService = new UserService();
        
        var register = new CqrsRegister();
        register.RegisterCommand<CreateUserCommand, CreateUserCommandHandler>();
        register.RegisterQuery<GetUserByIdQuery, GetUserByIdQueryHandler>();

        var instanceProvider = new LocalInstanceProvider();
        instanceProvider.RegisterFactory(() => new CreateUserCommandHandler(userService));
        instanceProvider.RegisterFactory(() => new GetUserByIdQueryHandler(userService));
        
        var cqrs = new CqrsService(
            register.GetCommandQueryResolver(), 
            instanceProvider);

        var user = new User(Guid.NewGuid().ToString())
        {
            Name = "myname",
            Email = "me@email.com"
        };
        
        var command = new CreateUserCommand(user);
        await cqrs.Handle(command);

        var query = new GetUserByIdQuery(user.Id);
        var result = await cqrs.Handle(query);

        if (!result.Equals(user)) throw new Exception("Models dont match");
    }
}