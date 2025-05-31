namespace Cqrs.Shared.Models;

public class User(string id)
{
    public string Id { get; } = id;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}