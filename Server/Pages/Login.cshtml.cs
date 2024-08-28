using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using CESistemaLogin.ServerApp.Server.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;

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

      [Required]
      [BindProperty]
      public LoginModel LoginModel { get; set; } = new();
      public string? ErrorMessage { get; set; }
      public bool? SendEmailConfirmation {get; set;} = false;

      public async Task<IActionResult> OnPostAsync(string? action)
      {
         // Si se presionó el botón "Reenviar Email"
         if (action == "resendEmail")
         {
            SendEmailConfirmation = null;
            ErrorMessage = "Ok Email de Verificación Reenviado.";
            return Page();
         }

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
         else if(response.StatusCode == HttpStatusCode.Unauthorized)
         {
            ErrorMessage = "Debes de verificar tu email primero.";  
            SendEmailConfirmation = true;
            return Page();
         }  
         // Si la autenticación falla, asigna un mensaje de error
         ErrorMessage = "Invalid login attempt. Please check your email and password.";
         return Page();
      }
   }
}
