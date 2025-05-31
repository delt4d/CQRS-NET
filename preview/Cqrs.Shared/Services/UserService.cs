using Cqrs.Shared.Models;

namespace Cqrs.Shared.Services;

public class UserService : IUserService
{
    private readonly List<User> _users = [];
    
    public Task<User> GetById(string id, CancellationToken? cancellationToken)
    {
        var result = _users.FirstOrDefault(x => x.Id == id)
               ?? throw new Exception($"User not found with id {id}.");
        return Task.FromResult(result);
    }

    public Task<User> GetByName(string name, CancellationToken? cancellationToken)
    {
        var result = _users.FirstOrDefault(x => x.Name == name)
                     ?? throw new Exception($"User not found with name {name}");
        return Task.FromResult(result);
    }

    public Task Save(User user, CancellationToken? cancellationToken)
    {
        if (_users.Any(x => x.Id == user.Id))
            throw new Exception($"A user with id {user.Id} already exists.");
        
        _users.Add(user);

        return Task.CompletedTask;
    }

    public Task Update(User user, CancellationToken? cancellationToken)
    {
        var index = _users.FindIndex(x => x.Id == user.Id);
        
        if (index == -1)
            throw new Exception($"A user with id {user.Id} was not found to update.");

        _users[index] = user;

        return Task.CompletedTask;
    }

    public Task Delete(string userId, CancellationToken? cancellationToken)
    {
        var user = _users.FirstOrDefault(x => x.Id == userId);

        if (user is null)
            throw new Exception($"A user with id {userId} was not found to delete.");
        
        _users.Remove(user);

        return Task.CompletedTask;
    }
}

public interface IUserService
{
    public Task<User> GetById(string id, CancellationToken? cancellationToken);
    public Task<User> GetByName(string name, CancellationToken? cancellationToken);
    public Task Save(User user, CancellationToken? cancellationToken);
    public Task Update(User user, CancellationToken? cancellationToken);
    public Task Delete(string userId, CancellationToken? cancellationToken);
}