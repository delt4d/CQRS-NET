using Cqrs.Shared.Models;

namespace Cqrs.Shared.Services;

public class UserService : IUserService
{
    private readonly List<User> _users = [];
    
    public Task<User> GetById(string id)
    {
        var result = _users.FirstOrDefault(x => x.Id == id)
               ?? throw new InvalidOperationException($"User not found with id {id}.");
        return Task.FromResult(result);
    }

    public Task Save(User user)
    {
        if (_users.Any(x => x.Id == user.Id))
            throw new InvalidOperationException($"A user with id {user.Id} already exists.");
        
        _users.Add(user);

        return Task.CompletedTask;
    }
}

public interface IUserService
{
    public Task<User> GetById(string id);
    public Task Save(User user);
}