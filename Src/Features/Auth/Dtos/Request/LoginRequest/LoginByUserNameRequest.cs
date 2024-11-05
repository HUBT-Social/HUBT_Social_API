﻿using System.ComponentModel.DataAnnotations;

namespace HUBT_Social_API.Features.Auth.Dtos.Request.LoginRequest;

public class LoginByUserNameRequest : ILoginRequest
{
    [Required] 
    public string UserName { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string Identifier => UserName;
}