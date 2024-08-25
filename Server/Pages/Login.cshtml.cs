using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using CESistemaLogin.ServerApp.Server.Authentication;
using System.Text.Json;
using System.Text;

namespace CESistemaLogin.ServerApp.Server.Pages
{
   public class PageLoginModel(IHttpClientFactory httpClientFactory) : PageModel
   {
      private readonly HttpClient _httpClient = httpClientFactory.CreateClient("TheLocalClient");

      [BindProperty]
      public LoginModel LoginModel { get; set; } = new();

      public async Task<IActionResult> OnPostAsync()
      {
         var json = JsonSerializer.Serialize(LoginModel);
         var content = new StringContent(json, Encoding.UTF8, "application/json");
         var response = await _httpClient.PostAsync("/account/login", content);
         //Codigo funcionando
         await Task.Delay(10000);
         return Page();
      }
   }
}
