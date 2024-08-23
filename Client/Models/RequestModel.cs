using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CESistemaLogin.ServerApp.Client.Models;

public class LoginRequest
{
   [Required]
   [EmailAddress] 
   public string UserEmail { get; set; } = string.Empty; 
   
   [Required][StringLength(16, MinimumLength = 8)]
   public string Password { get; set; } = string.Empty;
}

public class UserNameModel
{
   [Required]
   [EmailAddress] 
   public string UserEmail { get; set; } = string.Empty; 
}
