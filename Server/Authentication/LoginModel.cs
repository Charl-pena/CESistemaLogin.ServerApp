using System.ComponentModel.DataAnnotations;

namespace TBAnalisisFinanciero.Server.Authentication;

public class LoginModel
{
    [Required(ErrorMessage = "User name is required")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}

public class ValidateUsernameRequest
{
    [Required(ErrorMessage = "Enter a valid email address or phone number.")]
    public string UserName { get; set; } = string.Empty;
}

public class ValidateUsernameResponse
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
}
