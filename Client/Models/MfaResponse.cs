using System.Text.Json.Serialization;

namespace CESistemaLogin.Client.Models;
public class QrResponse
{
   [JsonPropertyName("token")] 
   public string? AccessToken { get; set; }

   [JsonPropertyName("qr")] 
   public string? QrString{ get; set; }
}
public class TokenResponse
{
   [JsonPropertyName("token")] 
   public string? AccessToken { get; set; }
}