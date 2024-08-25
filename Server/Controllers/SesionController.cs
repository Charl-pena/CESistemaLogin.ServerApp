using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using CESistemaLogin.ServerApp.Server.Authentication;

namespace CESistemaLogin.ServerApp.Server.Controllers;

[Route("[controller]")]
[Authorize]
public class SesionController(
   IHttpClientFactory httpClientFactory) : Controller
{
   private readonly HttpClient _httpClient = httpClientFactory.CreateClient("TheApiClient");

   [HttpPost("Login")]
   [AllowAnonymous]
   public async Task<IActionResult> Login([FromBody] LoginModel model)
   {
      Console.WriteLine("El klo de Monica esta gordo");
      return View();
   }
}