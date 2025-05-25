using Cqrs.Shared.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IUserService, UserService>();

var app = builder.Build();

app.Run();
