using System.ComponentModel.DataAnnotations;

namespace HUBT_Social_API.Features.Auth.Dtos.Request;

public class GetUserByUserNameRequest
{
    [Required] public string UserName { get; set; } = string.Empty;
}