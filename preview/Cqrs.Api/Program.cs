using System.Text.Json;
using Cqrs.Core;
using Cqrs.DependencyInjection;
using Cqrs.Shared.Commands;
using Cqrs.Shared.CommandsWithResults;
using Cqrs.Shared.Models;
using Cqrs.Shared.Queries;
using Cqrs.Shared.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddCqrs(opt => opt
    .SetInstanceProvider(sp => new DependencyInjectionInstanceProvider(sp))
    .InjectFromAssemblies(
        typeof(Program).Assembly,
        typeof(User).Assembly
    ));

var app = builder.Build();

app.MapGet("/query/id/{id}", async (HttpContext ctx, string id, ICqrsService container) =>
{
    var query = new GetUserByIdQuery(id);
    var result = await container.Handle(query);
    await ctx.Response.WriteAsJsonAsync(JsonSerializer.Serialize(result));
});

app.MapGet("/query/name/{name}", async (HttpContext ctx, string name, ICqrsService container) =>
{
    var query = new GetUserByNameQuery(name);
    var result = await container.Handle(query);
    await ctx.Response.WriteAsJsonAsync(JsonSerializer.Serialize(result));
});

app.MapGet("/command/create-user/{id}", async (HttpContext ctx, string id, ICqrsService container) =>
{
    var user = new User(id)
    {
        Name = "myname",
        Email = "me@email.com"
    };
    var command = new CreateUserCommand(user);
    await container.Handle(command);
    await ctx.Response.CompleteAsync();
});

app.MapGet("/command/create-user", async (HttpContext ctx, ICqrsService container) =>
{
    var user = new User(Guid.NewGuid().ToString())
    {
        Name = "mynameforresult",
        Email = "meforresult@email.com"
    };
    var command = new CreateUserCommandWithResult(user);
    var createUserResult = await container.Handle(command);
    await ctx.Response.WriteAsJsonAsync(createUserResult);
});

app.MapGet("/command/update-user/{id}", async (HttpContext ctx, string id, ICqrsService container) =>
{
    var user = new User(id)
    {
        Name = "updatename",
        Email = "update@email.com"
    };
    var command = new UpdateUserCommand(user);
    await container.Handle(command);
    await ctx.Response.CompleteAsync();
});

app.MapGet("/command/delete-user/{id}", async (HttpContext ctx, string id, ICqrsService container) =>
{
    var command = new DeleteUserCommand(id);
    await container.Handle(command);
    await ctx.Response.CompleteAsync();
});

app.Run();