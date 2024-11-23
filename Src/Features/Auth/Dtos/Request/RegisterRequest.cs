﻿using System.ComponentModel.DataAnnotations;

namespace HUBT_Social_API.Features.Auth.Dtos.Request;

public class RegisterRequest
{
    [Required] public string UserName { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "PassWord must be alike")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required][EmailAddress] public string Email { get; set; } = string.Empty;
}