namespace HUBT_Social_API.Features.Auth.Dtos.Reponse;

public class AuthResponse(string message, object? data = null)
{
    public string? Message { get; set; } = message;
    public object? Data { get; set; } = data;
}