using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Text;

using CESistemaLogin.ServerApp.Server.Authentication;

namespace CESistemaLogin.ServerApp.Server.Controllers;

/*
OJO Hay que actualizar las rutas permitidas en el MIddleware de autenticación si se agregar una nueva ruta publica
*/
[ApiController]
[Route("[controller]")]
[Authorize]
public class AccountController(
   IHttpClientFactory httpClientFactory) : ControllerBase
{
   private readonly HttpClient _httpClient = httpClientFactory.CreateClient("TheApiClient");

   [HttpPost("register")]
   [AllowAnonymous]
   public async Task<IActionResult> Register([FromBody] RegisterModel model)
   {
      var result = await ForwardRequestAsync("/ServerApp/register", model);
      return result;
   }
   [HttpPost("reset-password")]
   [AllowAnonymous]
   public async Task<IActionResult> ResetPassword([FromBody] UserNameModel model)
   {
      var result = await ForwardRequestAsync("/ServerApp/reset-password", model);
      return result;
   }
   [HttpPost("login")]
   [AllowAnonymous]
   public async Task<IActionResult> Login([FromBody] LoginModel model)
   {
      var result = await ForwardRequestAsync("/ServerApp/login", model);
      return result;
   }

   [HttpPost("logout")]
   public async Task<IActionResult> Logout([FromBody] UserNameModel model)
   {
      try
      {
         model.UserEmail = model.UserEmail ;
         // Elimina la cookie de autenticación
         await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
         
         return Ok(new { message = "Logout successful" });
      }
      catch (Exception ex)
      {
         // Si ocurre algún error, regresa un código 500 Internal Server Error con el detalle del error
         return StatusCode(500, new { error = "Logout failed", details = ex.Message });
      }
   }

   [HttpPost("resend-email")]
   [AllowAnonymous]
   public async Task<IActionResult> ResendEmail([FromBody] UserNameModel model)
   {
      var result = await ForwardRequestAsync("/admin/resend-email", model);
      return result;
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
         var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
         identity.AddClaim(new Claim(ClaimTypes.Name, model.UserEmail));
         identity.AddClaim(new Claim(ClaimTypes.Role, AppRoles.User));
         
         var principal = new ClaimsPrincipal(identity);
         //agregamos la cookie
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

      try
      {
         var jsonContent = new StringContent(
             JsonSerializer.Serialize(model),
             Encoding.UTF8,
             "application/json"
         );
         if (headerAuth != null)
         {
            _httpClient.DefaultRequestHeaders.Authorization =
               new AuthenticationHeaderValue(headerAuth[0], headerAuth[1]);
         }

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