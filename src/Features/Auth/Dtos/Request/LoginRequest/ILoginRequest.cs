namespace HUBT_Social_API.src.Features.Auth.Dtos.Request.LoginRequest
{
    public interface ILoginRequest
    {
        string Identifier { get; }
        string Password { get; }
    }
}
