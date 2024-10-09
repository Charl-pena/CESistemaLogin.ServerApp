using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

using CESistemaLogin.ServerApp.Server.Authentication;

namespace CESistemaLogin.ServerApp.Server.Pages
{
   public class RegisterPageModel(IHttpClientFactory httpClientFactory) : PageModel
   {
      private readonly HttpClient _httpClient = httpClientFactory.CreateClient("TheLocalClient");
      
      [Required]
      [BindProperty]
      public RegisterModel RegisterModel { get; set; } = new();
      // Propiedad para el mensaje de error
      public string? ErrorMessage { get; set; }
      public string? SuccessMessage { get; set; }

      public async Task<IActionResult> OnPostAsync()
      {
         var response = await _httpClient.PostAsJsonAsync("/account/register", RegisterModel);
         if (response.IsSuccessStatusCode)
         {
            var confirmationLink = await response.Content.ReadFromJsonAsync<TokenResponse>();
            if (confirmationLink != null)
            {
               // var mfaResponse = await response.Content.ReadAsStringAsync();
               SuccessMessage = confirmationLink.AccessToken;
               ErrorMessage = null;
               return Page();
            }
         }
         // Si la autenticación falla, asigna un mensaje de error
         ErrorMessage = "Invalid registration attempt. Please try again at a later time.";
         return Page();
      }
   }
}
