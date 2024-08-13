using System.Text.Json.Serialization;

namespace TBAnalisisFinanciero.Client.Models;
public class TokenResponse
{
   [JsonPropertyName("token")] 
   public string? AccessToken { get; set; }

   // [JsonPropertyName("refreshToken")] 
   // public string? RefreshToken { get; set; }
}

// public class TokenResponse
// {
//    [JsonPropertyName("token")]
//    public TokenDetails? Token { get; set; }
// }

// public class TokenDetails
// {
//    [JsonPropertyName("result")]
//    public string? Result { get; set; }
//    public int Id { get; set; }
//    public object? Exception { get; set; }
//    public int Status { get; set; }
//    public bool IsCanceled { get; set; }
//    public bool IsCompleted { get; set; }
//    public bool IsCompletedSuccessfully { get; set; }
//    public int CreationOptions { get; set; }
//    public object? AsyncState { get; set; }
//    public bool IsFaulted { get; set; }
// }