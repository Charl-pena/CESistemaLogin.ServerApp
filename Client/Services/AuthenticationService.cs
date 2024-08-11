using TBAnalisisFinanciero.Client.Models;
using System.Net.Http.Json;

namespace TBAnalisisFinanciero.Client.Services;

public class AuthenticationService : IAuthenticationService
{
   // Inject the HttpClient into the constructor      
   private readonly HttpClient _httpClient; 
   public AuthenticationService(HttpClient httpClient)
   { 
      _httpClient = httpClient; 
   }
   public async Task<TokenResponse> LoginUserAsync(LoginRequest requestModel)
   {
      var response = await _httpClient.PostAsJsonAsync("account/login", requestModel); 
      if (response.IsSuccessStatusCode)
      { 
         var loginResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
         if(loginResponse == null)
         {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(); 
            Console.WriteLine(error); 
            throw new Exception(error?.Message ?? "Error al leer la respuesta de la API");
         }
         return loginResponse; 
      }
      else
      {
         var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(); 
         Console.WriteLine(error); 
         throw new Exception(error?.Message ?? "Error al leer la respuesta de la API");            
         // TODO: Handle the error in a proper way
      }
   }
}