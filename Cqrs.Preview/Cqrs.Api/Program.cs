using System.Text.Json;
using Cqrs.Core;
using Cqrs.DependencyInjection;
using Cqrs.Shared.Commands;
using Cqrs.Shared.Models;
using Cqrs.Shared.Queries;
using Cqrs.Shared.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddCqrs(opt => opt
        .SetInstanceProvider(sp => new DependencyInjectionInstanceProvider(sp))
        .InjectFromAssemblies(
            typeof(Program).Assembly,
            typeof(User).Assembly));

var app = builder.Build();
var container = app.Services.GetRequiredService<ICqrsService>();

app.MapGet("/query/{id}", async (HttpContext ctx, string id) =>
{ 
    var query = new GetUserByIdQuery(id); 
    var result = await container.Handle(query);
    await ctx.Response.WriteAsJsonAsync(JsonSerializer.Serialize(result));
});

app.Map("/command/{id}", async (HttpContext ctx, string id) =>
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

app.MapGet("/query/di/{id}", async (HttpContext ctx, string id) =>
{ 
    var query = new GetUserByIdQuery(id);
    var handler = app.Services.GetRequiredService<GetUserByIdQueryHandler>();
    var result = await handler.Handle(query);
    await ctx.Response.WriteAsJsonAsync(JsonSerializer.Serialize(result));
});

app.Map("/command/di/{id}", async (HttpContext ctx, string id) =>
{
    var user = new User(id)
    {
        Name = "myname",
        Email = "me@email.com"
    };
    var command = new CreateUserCommand(user);
    var handler = app.Services.GetRequiredService<CreateUserCommandHandler>();
    await handler.Handle(command);
    await ctx.Response.CompleteAsync();
});

app.Run();
