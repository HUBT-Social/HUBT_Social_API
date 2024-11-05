using System.ComponentModel.DataAnnotations;

namespace HUBT_Social_API.Features.Auth.Dtos.Request;

public class SearchUserByUserNameOrPasswordRequest
{
    [Required] 
     public string UserNameOrEmail { get; set;} = string.Empty;


}