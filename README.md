# CQRS-NET
### Implementation of Command Query Responsibility Segregation Pattern in C#.

<br/>

#### Create Commands, Queries and Handlers.

Command and CommandHandler example:
```csharp
public record SendMessageCommand(long RoomId, string Message, UserModel User) : ICommand;

public class SendMessageCommandHandler(IChatService chatService) : ICommandHandler<SendMessageCommand> 
{
    public async Task Handle(CreateUserCommand command, CancellationToken? cancellationToken)
    {
        ...
    }
}
```

Query and QueryHandler example:
```csharp
public record GetAllUsersConnectedQuery(long RoomId) : IQuery<Task<IEnumerable<UserModel>>>;

public class GetAllUsersConnectedQueryHandler(IChatService chatService, IUserService userService) : IQueryHandler<GetAllUsersConnectedQuery, Task<IEnumerable<UserModel>>>
{
    public Task<IEnumerable<UserModel>> Handle(GetAllUsersConnectedQuery query, CancellationToken? cancellationToken)
    {
        ...
    }
}
```

#### Manual CQRS service configuration.

Example of CqrsService configuration:
```csharp
public CqrsService ConfigureCqrs(IUserService userService, IChatService chatService) 
{
    // Define commands, queries and their handlers
    CqrsRegister register = new CqrsRegister();
    register.RegisterCommand<ConnectToRoomCommand, ConnectToRoomCommandHandler>();
    register.RegisterCommand<SendMessageCommand, SendMessageCommandHandler>();
    register.RegisterQuery<GetAllUsersConnectedQuery, GetAllUsersConnectedQueryHandler>();
    register.RegisterQuery<GetMessagesHistoryQuery, GetMessagesHistoryQueryHandler>();
    
    // Define how handlers should be instantiated
    LocalInstanceProvider localInstanceProvider = new LocalInstanceProvider();
    localInstanceProvider.RegisterFactory(() => new ConnectToRoomCommandHandler(chatService));
    localInstanceProvider.RegisterFactory(() => new SendMessageCommandHandler(chatService));
    localInstanceProvider.RegisterFactory(() => new GetAllUsersConnectedQueryHandler(chatService, userService));
    localInstanceProvider.RegisterFactory(() => new GetMessagesHistoryQueryHandler(chatService, userService));
    
    // Get the command-query handlers resolver
    CqrsCommandQueryResolver resolver = register.BuildCommandQueryResolver();
    
    // Create CQRS service
    return new CqrsService(resolver, (IInstanceProvider)localInstanceProvider);
}
```

Usage example:
```csharp
CqrsService container = ConfigureCqrs(...);

await container.Handle(new ConnectToRoomCommandHandler(...));
await container.Handle(new SendMessageCommandHandler(...));
await container.Handle(new GetAllUsersConnectedQueryHandler(...));
await container.Handle(new GetMessagesHistoryQueryHandler(...));
```

#### Dependency Injection.

Configure CQRS service:
```csharp
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IChatService, ChatService>();

// Inject all commands, queries and handlers from current assembly
builder.Services.AddCqrs(opt => opt.InjectFromAssembly(typeof(Program).Assembly));
```

Get CQRS service from registered services collection:
```csharp
var app = builder.Build();
var container = app.Services.GetRequiredService<ICqrsService>();
```

CQRS service injection in controller method:
```csharp
public class ChatController(ICqrsService container) : ControllerBase 
{
    [HttpPost("room/{id}/send-message")]
    public async Task<IActionResult> SendMessage(long id, SendMessageIn data) 
    {
        var command = new SendMessageCommand(id, data.Message, data.User);
        await container.Handle(command);
        return Ok();
    }
    
    [HttpGet("room/{id}/messages")]
    public async Task<ActionResult<IEnumerable<GetMessagesHistoryOut>>> GetMessagesHistory(long id) 
    {
        var query = new GetMessagesHistory(id);
        var results = new List<GetMessagesHistoryOut>();
    
        await foreach (var message in container.Handle(query)) 
        {
            results.Add(new GetMessagesHistoryOut(message));
        }
    
        return Ok(results);
    }
    
    ...
}
```

Handlers can be injected directly:
```csharp
public class ChatController(GetAllUsersConnectedQueryHandler getUsersConnectedHandler) : ControllerBase 
{
    [HttpGet("room/{id}/users")]
    public async Task<ActionResult<IEnumerable<GetUsersConnectedOut>>> GetUsersInRoom(long id) 
    {
        var query = new GetAllUsersConnectedQuery(id);
        var results = new List<GetUsersConnectedOut>();
        
        await foreach (var userConnected in getUsersConnectedHandler.Handle(query))
        {
            results.Add(new GetUsersConnectedOut(userConnected));
        }
        
        return Ok(results);
    }
    
    ...
}
```