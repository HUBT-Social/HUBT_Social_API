using HUBT_Social_API.Features.Chat.Services.Interfaces;

public class UserConnectionManager : IUserConnectionManager
{
    private readonly Dictionary<string, string> _userConnections = new();

    public void AddConnection(string userName, string connectionId)
    {
        lock (_userConnections)
        {
            _userConnections[userName] = connectionId;
        }
    }

    public void RemoveConnection(string userName)
    {
        lock (_userConnections)
        {
            if (_userConnections.ContainsKey(userName)) _userConnections.Remove(userName);
        }
    }

    public string? GetConnectionId(string userName)
    {
        lock (_userConnections)
        {
            _userConnections.TryGetValue(userName, out var connectionId);
            return connectionId;
        }
    }
}