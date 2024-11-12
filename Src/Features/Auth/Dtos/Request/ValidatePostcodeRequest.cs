namespace HUBT_Social_API.Features.Auth.Dtos.Request;

public class ValidatePostcodeRequest
{
    public string Postcode { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string UserAgent { get; set; } = string.Empty;

}