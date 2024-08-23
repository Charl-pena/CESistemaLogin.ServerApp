using System.ComponentModel.DataAnnotations;

namespace CESistemaLogin.ServerApp.Server.Authentication;

public class LoginModel
{
    [Required(ErrorMessage = "User name is required")]
    public string UserEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}

public class UserNameModel
{
    [Required(ErrorMessage = "User name is required")]
    public string UserEmail { get; set; } = string.Empty;
}
