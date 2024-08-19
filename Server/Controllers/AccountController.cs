using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Text;

using TBAnalisisFinanciero.Server.Authentication;

namespace TBAnalisisFinanciero.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController(
   IHttpClientFactory httpClientFactory) : ControllerBase
{
   private readonly HttpClient _httpClient = httpClientFactory.CreateClient("TheApiClient");

   [HttpPost("api-mfa")]
   [Authorize]
   public async Task<IActionResult> MfaStatus([FromBody] UserNameModel model)
   {
      return await ForwardRequestAsync("/Account/mfa", model);
   }

   [HttpPost("api-set-mfa")]
   [Authorize]
   public async Task<IActionResult> SetMfa([FromBody] LoginModel model)
   {
      return await ForwardRequestAsync("/Account/set-mfa", model);
   }

   [HttpPost("api-check-mfa-key")]
   [Authorize]
   public async Task<IActionResult> CheckMfaKey([FromBody] LoginModel model)
   {
      var result = await ForwardRequestAsync("/Account/check-mfa-key", model);
      // Verificar si el resultado fue un OkObjectResult
      // if (result is OkObjectResult okResult)
      if (result is OkObjectResult)
      {
         //agregamos la cookie
         var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
         identity.AddClaim(new Claim(ClaimTypes.Name, model.UserName));
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