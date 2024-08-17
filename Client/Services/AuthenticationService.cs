using TBAnalisisFinanciero.Client.Models;
using System.Net.Http.Json;
using System.Net;

namespace TBAnalisisFinanciero.Client.Services;

// public class AuthenticationService : IAuthenticationService
public class AuthenticationService
{
   // Inject the HttpClient into the constructor      
   private readonly HttpClient _httpClient; 
   public AuthenticationService(HttpClient httpClient)
   { 
      _httpClient = httpClient; 
   }

   public async Task<QrResponse?> MfaStatusAsync(UserNameModel requestModel)
   {
         /*No neceseramiente, puedo utilizar el mismo mecanismo de localstorage, solo necesito 
         actualizar el valor una vez se haya terminado la verificacion del usuario*/
      
      var response = await _httpClient.PostAsJsonAsync("Account/api-mfa", requestModel); 
      if (response.IsSuccessStatusCode)
      {
         var mfaResponse = await response.Content.ReadFromJsonAsync<QrResponse>();
         if(mfaResponse == null)
         {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(); 
            Console.WriteLine(error); 
            throw new Exception(error?.Message ?? "Error al leer la respuesta de la API");
         }
         return mfaResponse; 
      }
      else if(response.StatusCode == HttpStatusCode.NotFound)
      {
         //Estamos utilizando el token vacio como indicador de que ya tiene el mfa habilitado
         return new QrResponse(){AccessToken = ""};
      }
      return null;
   }
   public async Task<bool?> SetMfaAsync(LoginRequest requestModel)
   {
         /*No neceseramiente, puedo utilizar el mismo mecanismo de localstorage, solo necesito 
         actualizar el valor una vez se haya terminado la verificacion del usuario*/
      
      var response = await _httpClient.PostAsJsonAsync("Account/api-set-mfa", requestModel); 
      if (response.IsSuccessStatusCode)
      {
         // var loginResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
         // if(loginResponse == null)
         // {
         //    var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(); 
         //    Console.WriteLine(error); 
         //    throw new Exception(error?.Message ?? "Error al leer la respuesta de la API");
         // }
         return true; 
      }
      else if(response.StatusCode == HttpStatusCode.NotFound)
      {
         // var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(); 
         // return new TokenResponse(){AccessToken = ""};
         // throw new Exception(error?.Message ?? "Error al leer la respuesta de la API");            
         // throw new Exception("Error al leer la respuesta de la API");            
         // TODO: Handle the error in a proper way

         return false;
      }
      return null;
   }
   public async Task<TokenResponse?> CheckMfaKeyAsync(LoginRequest requestModel)
   {
         /*No neceseramiente, puedo utilizar el mismo mecanismo de localstorage, solo necesito 
         actualizar el valor una vez se haya terminado la verificacion del usuario*/
      
      var response = await _httpClient.PostAsJsonAsync("Account/api-check-mfa-key", requestModel); 
      if (response.IsSuccessStatusCode)
      {
         var mfaResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
         if(mfaResponse == null)
         {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(); 
            Console.WriteLine(error); 
            throw new Exception(error?.Message ?? "Error al leer la respuesta de la API");
         }
         return mfaResponse;  
      }
      else if(response.StatusCode == HttpStatusCode.NotFound)
      {
         // var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(); 
         // return new TokenResponse(){AccessToken = ""};
         // throw new Exception(error?.Message ?? "Error al leer la respuesta de la API");            
         // throw new Exception("Error al leer la respuesta de la API");            
         // TODO: Handle the error in a proper way

         return null;
      }
      return null;
   }
}