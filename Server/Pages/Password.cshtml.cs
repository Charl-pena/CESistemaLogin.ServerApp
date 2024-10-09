using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

using CESistemaLogin.ServerApp.Server.Authentication;
using System.Net;

namespace CESistemaLogin.ServerApp.Server.Pages
{
   public class PasswordPageModel(IHttpClientFactory httpClientFactory) : PageModel
   {
      private readonly HttpClient _httpClient = httpClientFactory.CreateClient("TheLocalClient");
      
      [Required]
      [BindProperty]
      public UserNameModel UserNameModel { get; set; } = new();
      // Propiedad para el mensaje de error
      public string? ErrorMessage { get; set; }
      public string? SuccessMessage { get; set; }

     
      [FromQuery(Name = "token")]
      public string? Token { get; set; }
      
      public void OnGet()
      {
         // if(!String.IsNullOrEmpty(Token)){
            
         // }
      }

      public async Task<IActionResult> OnPostAsync()
      {
         var response = await _httpClient.PostAsJsonAsync("/account/reset-password", UserNameModel);
         if (response.IsSuccessStatusCode)
         {
            var resetLink = await response.Content.ReadFromJsonAsync<TokenResponse>();
            if (resetLink != null)
            {
               // Console.WriteLine(resetLink.AccessToken);
               SuccessMessage = resetLink.AccessToken;
               ErrorMessage = null;
            }
         }
         else if(response.StatusCode == HttpStatusCode.Unauthorized)
         {
            ErrorMessage = "Nonexistent user check email.";
         }else
         {
            // Si la autenticación falla por razón desconocida, asigna un mensaje de error Generico
            ErrorMessage = "Unsuccessful request, please try again later.";
         }
         return Page();
      }
   }
}
