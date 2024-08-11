using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TBAnalisisFinanciero.Client.Models;

public class LoginRequest
{
   [Required]
   [EmailAddress] 
   [JsonPropertyName("username")]
   public string Username { get; set; } = string.Empty; 
   
   [Required][StringLength(16, MinimumLength = 8)]
   [JsonPropertyName("password")] 
   public string Password { get; set; } = string.Empty;
}
