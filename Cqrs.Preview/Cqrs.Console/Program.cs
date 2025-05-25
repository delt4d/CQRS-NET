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
    private static readonly UserService UserService = new ();
    
    static async Task Main(string[] args)
    {
        try
        {
            // Register all commands and queries, and its handlers
            var register = new CqrsRegister();
            register.RegisterCommand<CreateUserCommand, CreateUserCommandHandler>();
            register.RegisterCommand<UpdateUserCommand, UpdateUserCommandHandler>();
            register.RegisterCommand<DeleteUserCommand, DeleteUserCommandHandler>();
            register.RegisterQuery<GetUserByIdQuery, GetUserByIdQueryHandler>();
            register.RegisterQuery<GetUserByNameQuery, GetUserByNameQueryHandler>();
            System.Console.WriteLine("Handlers for commands and queries registered.");

            // create the command query resolver with the 
            // registered commands and queries
            var commandQueryResolver = register.BuildCommandQueryResolver();

            /*
             * ActivatorInstanceProvider is an alternative that uses
             * System.Activator to create a instance of the class
             * var instanceProvider = new ActivatorInstanceProvider();
             */
            var instanceProvider = new LocalInstanceProvider();

            // Register the way that handlers must be instantiated
            instanceProvider.RegisterFactory(() => new CreateUserCommandHandler(UserService));
            instanceProvider.RegisterFactory(() => new UpdateUserCommandHandler(UserService));
            instanceProvider.RegisterFactory(() => new DeleteUserCommandHandler(UserService));
            instanceProvider.RegisterFactory(() => new GetUserByIdQueryHandler(UserService));
            instanceProvider.RegisterFactory(() => new GetUserByNameQueryHandler(UserService));
            System.Console.WriteLine("Instance provider configured.");

            // create CQRS service
            var container = new CqrsService(
                commandQueryResolver,
                instanceProvider);

            var user = CreateUser("myname", "me@email.com");

            // Test user creation
            await container.Handle(new CreateUserCommand(user));
            await container.Handle(new CreateUserCommand(CreateUser("myname2", "me2@email.com")));
            await container.Handle(new CreateUserCommand(CreateUser("myname3", "me3@email.com")));
            _ = await container.Handle(new GetUserByIdQuery(user.Id)) ?? throw new Exception("Wrong user found");
            System.Console.WriteLine("Users created.");

            // Test user update
            var updateUser = CreateUser("updatedname", "updated@email.com", user.Id);
            await container.Handle(new UpdateUserCommand(updateUser));
            _ = await container.Handle(new GetUserByNameQuery(updateUser.Name)) ??
                throw new Exception("Wrong updated_user found");
            System.Console.WriteLine("User updated.");

            // Test user delete
            await container.Handle(new DeleteUserCommand(user.Id));
            await ExpectToThrow(() => container.Handle(new GetUserByIdQuery(user.Id)), "Expected user to be deleted");
            System.Console.WriteLine("User deleted.");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Unexpected exception: {ex.Message}");
        }
    }

    private static async Task ExpectToThrow(Func<Task> action, string errorMessage)
    {
        try
        {
            await action.Invoke();
            throw new Exception(errorMessage);
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Expected exception was thrown: {ex.Message}");
        }
    }

    private static User CreateUser(string name, string email, string? id = null)
    {
        return new User(id ?? Guid.NewGuid().ToString())
        {
            Name = name,
            Email = email
        };
    }
}