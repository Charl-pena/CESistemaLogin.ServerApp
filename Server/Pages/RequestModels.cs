using System.Text.Json.Serialization;

namespace CESistemaLogin.ServerApp.Server.Pages;

public class TokenResponse
{
   [JsonPropertyName("token")] 
   public string? AccessToken { get; set; }
}