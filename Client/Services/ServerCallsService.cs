using CESistemaLogin.Client.Models;
using System.Net.Http.Json;
using System.Net;

namespace CESistemaLogin.Client.Services;

// public class ServerCallsService : IServerCallsService
public class ServerCallsService
{
   private readonly HttpClient _httpClient; 
   public ServerCallsService(HttpClient httpClient)
   { 
      _httpClient = httpClient; 
   }

   public async Task<bool> Logout(UserNameModel requestModel)
   {
      var response = await _httpClient.PostAsJsonAsync("Account/Logout", requestModel); 
      return response.IsSuccessStatusCode;
   }

   public async Task<QrResponse?> MfaStatusAsync(UserNameModel requestModel)
   {
      var response = await _httpClient.PostAsJsonAsync("Account/api-mfa", requestModel); 
      if (response.IsSuccessStatusCode)
      {
         //Se espera que regrese el Qr
         var mfaResponse = await response.Content.ReadFromJsonAsync<QrResponse>();
         if(mfaResponse == null)
         {
            return null;
         }
         return mfaResponse; 
      }
      else if((int)response.StatusCode == 769)
      {
         //Estamos utilizando el token vacio como indicador de que ya tiene el mfa habilitado
         return new QrResponse(){AccessToken = ""};
      }
      return null;
   }
   public async Task<bool?> SetMfaAsync(LoginRequest requestModel)
   {
      var response = await _httpClient.PostAsJsonAsync("Account/api-set-mfa", requestModel); 
      if (response.IsSuccessStatusCode)
      {
         return true; 
      }else if((int)response.StatusCode == 848)
      {
         return false;
      }
      return null;
   }
   public async Task<TokenResponse?> CheckMfaKeyAsync(LoginRequest requestModel)
   {
      var response = await _httpClient.PostAsJsonAsync("Account/api-check-mfa-key", requestModel); 
      if (response.IsSuccessStatusCode)
      {
         var mfaResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
         if(mfaResponse == null)
         {
            return null;
            // var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(); 
            // Console.WriteLine(error); 
            // throw new Exception(error?.Message ?? "Error al leer la respuesta de la API");
         }
         return mfaResponse;  
      }
      else if((int)response.StatusCode == 848)
      {
         return new TokenResponse() {AccessToken = String.Empty};
      }
      return null;
   }
}