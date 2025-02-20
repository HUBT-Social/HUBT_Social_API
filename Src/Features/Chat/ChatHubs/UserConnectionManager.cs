using HUBT_Social_API.Features.Chat.Services.Interfaces;

public class UserConnectionManager : IUserConnectionManager
{
    private readonly Dictionary<string, string> _userConnections = new();

    public void AddConnection(string userId, string connectionId)
    {
        lock (_userConnections)
        {
            _userConnections[userId] = connectionId;
        }
    }

    public void RemoveConnection(string userId)
    {
        lock (_userConnections)
        {
            if (_userConnections.ContainsKey(userId))
            {
                _userConnections.Remove(userId);
            }
        }
    }

    public string? GetConnectionId(string userId)
    {
        lock (_userConnections)
        {
            _userConnections.TryGetValue(userId, out var connectionId);
            return connectionId;
        }
    }
}