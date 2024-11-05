namespace HUBT_Social_API.Features.Auth.Dtos.Reponse;

public class AuthResponse(bool success,string message, object? data = null)
{
    public bool Success { get; set; } = success;
    public string? Message { get; set; } = message;
    public object? Data { get; set; } = data;
}