using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using CESistemaLogin.ServerApp.Server.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CESistemaLogin.ServerApp.Server.Pages
{
   public class PageLoginModel(IHttpClientFactory httpClientFactory) : PageModel
   {
      class TokenResponse
      {
         [JsonPropertyName("token")] 
         public string? AccessToken { get; set; }
      }
      private readonly HttpClient _httpClient = httpClientFactory.CreateClient("TheLocalClient");

      [BindProperty]
      [Required]
      public LoginModel LoginModel { get; set; } = new();
      // Propiedad para el mensaje de error
      public string? ErrorMessage { get; set; }
      public async Task<IActionResult> OnPostAsync()
      {
         var response = await _httpClient.PostAsJsonAsync("/account/login", LoginModel);
         if (response.IsSuccessStatusCode)
         {
            var mfaResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
            if(mfaResponse != null)
            {
               var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
               identity.AddClaim(new Claim(ClaimTypes.Name, LoginModel.UserEmail));
               identity.AddClaim(new Claim(ClaimTypes.Role, AppRoles.UserNoMFA));
               
               var principal = new ClaimsPrincipal(identity);
               //agregamos la cookie
               await HttpContext.SignInAsync(
                  CookieAuthenticationDefaults.AuthenticationScheme,
                  principal,
                  new AuthenticationProperties
                  {
                        IsPersistent = false,
                        AllowRefresh = true,
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(7)
                  });
               return Redirect($"/?token={mfaResponse.AccessToken}");
            }
         }
         // Si la autenticación falla, asigna un mensaje de error
         ErrorMessage = "Invalid login attempt. Please check your email and password.";
         return Page();
      }
   }
}
