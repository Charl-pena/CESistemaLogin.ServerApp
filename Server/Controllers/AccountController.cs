using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Text;

using CESistemaLogin.Server.Authentication;

namespace CESistemaLogin.Server.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class AccountController(
   IHttpClientFactory httpClientFactory) : ControllerBase
{
   private readonly HttpClient _httpClient = httpClientFactory.CreateClient("TheApiClient");

   [HttpPost("logout")]
   // [ValidateAntiForgeryToken]
   public async Task<IActionResult> Logout([FromBody] UserNameModel model)
   {
      try
      {
         model.UserEmail = model.UserEmail ;
         // Elimina la cookie de autenticación
         await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
         // Si se completa exitosamente, regresa un código 200 OK
         return Ok(new { message = "Logout successful" });
      }
      catch (Exception ex)
      {
         // Si ocurre algún error, regresa un código 500 Internal Server Error con el detalle del error
         return StatusCode(500, new { error = "Logout failed", details = ex.Message });
      }
   }

   [HttpPost("api-mfa")]
   public async Task<IActionResult> MfaStatus([FromBody] UserNameModel model)
   {
      return await ForwardRequestAsync("/ServerApp/mfa", model);
   }

   [HttpPost("api-set-mfa")]
   public async Task<IActionResult> SetMfa([FromBody] LoginModel model)
   {
      return await ForwardRequestAsync("/ServerApp/set-mfa", model);
   }

   [HttpPost("api-check-mfa-key")]
   public async Task<IActionResult> CheckMfaKey([FromBody] LoginModel model)
   {
      var result = await ForwardRequestAsync("/ServerApp/check-mfa-key", model);
      // if (result is OkObjectResult okResult)
      if (result is OkObjectResult)
      {
         //agregamos la cookie
         var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
         identity.AddClaim(new Claim(ClaimTypes.Name, model.UserEmail));
         identity.AddClaim(new Claim(ClaimTypes.Role, AppRoles.User));
         
         var principal = new ClaimsPrincipal(identity);
         await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                  IsPersistent = true,
                  AllowRefresh = true,
                  ExpiresUtc = DateTime.UtcNow.AddDays(1)
            });
         // Puedes acceder a los datos devueltos en okResult.Value
         // var data = okResult.Value; // Aquí puedes hacer lo que necesites con los datos
      }
      return result;
   }

   private async Task<IActionResult> ForwardRequestAsync(string uri, object model)
   {
      if (!ModelState.IsValid)
      {
         return BadRequest(ModelState);
      }

      var headerAuth = Request.Headers.Authorization.FirstOrDefault()?.Split(" ");
      if (headerAuth == null)
      {
         return Unauthorized("Authorization header is missing or invalid.");
      }

      try
      {
         var jsonContent = new StringContent(
             JsonSerializer.Serialize(model),
             Encoding.UTF8,
             "application/json"
         );

         _httpClient.DefaultRequestHeaders.Authorization =
             new AuthenticationHeaderValue(headerAuth[0], headerAuth[1]);

         var response = await _httpClient.PostAsync(uri, jsonContent);

         if (response.IsSuccessStatusCode)
         {
            var responseData = await response.Content.ReadAsStringAsync();
            return Ok(responseData);
         }
         else
         {
            return StatusCode((int)response.StatusCode, response.ReasonPhrase);
         }
      }
      catch (Exception ex)
      {
         return StatusCode(500, $"Error al comunicarse con la API: {ex.Message}");
      }
   }
}