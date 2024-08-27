using System.ComponentModel.DataAnnotations;

namespace CESistemaLogin.ServerApp.Server.Authentication;

public class LoginModel
{
   [Required(ErrorMessage = "Email is required")]
   [EmailAddress]
   public string UserEmail { get; set; } = string.Empty;

   [Required(ErrorMessage = "Password is required")]
   [DataType(DataType.Password)]
   public string Password { get; set; } = string.Empty;
}

public class RegisterModel
{
   [Required]
   public string Username { get; set; } = string.Empty;
   [Required]
   [EmailAddress]
   public string Email { get; set; } = string.Empty;

   [Required]
   [DataType(DataType.Password)]
   public string Password { get; set; }= string.Empty;

   [Required]
   [DataType(DataType.Password)]
   [Display(Name = "Confirm Password")]
   [Compare("Password", ErrorMessage = "Passwords don't match.")]
   public string ConfirmPassword { get; set; } = string.Empty;
}

public class UserNameModel
{
   [Required(ErrorMessage = "User name is required")]
   public string UserEmail { get; set; } = string.Empty;
}

