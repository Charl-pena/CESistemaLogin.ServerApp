using TBAnalisisFinanciero.Client.Models;
using System.Net.Http.Json;
using System.Net;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

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

   public async Task<TokenResponse> MfaStatusAsync(UserNameModel requestModel)
   {
         /*No neceseramiente, puedo utilizar el mismo mecanismo de localstorage, solo necesito 
         actualizar el valor una vez se haya terminado la verificacion del usuario*/
      
      var response = await _httpClient.PostAsJsonAsync("Account/api-mfa", requestModel); 
      if (response.IsSuccessStatusCode)
      {
         var loginResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
         if(loginResponse == null)
         {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(); 
            Console.WriteLine(error); 
            throw new Exception(error?.Message ?? "Error al leer la respuesta de la API");
         }
         Console.WriteLine(loginResponse.AccessToken); 
         return loginResponse; 
      }
      else if(response.StatusCode == HttpStatusCode.NotFound)
      {
         // var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(); 
         Console.WriteLine("Le llenare a monica la cara con semen"); 
         // return new TokenResponse(){AccessToken = ""};
         // throw new Exception(error?.Message ?? "Error al leer la respuesta de la API");            
         // throw new Exception("Error al leer la respuesta de la API");            
         // TODO: Handle the error in a proper way
      }
      return new TokenResponse(){AccessToken = ""};

   }

}