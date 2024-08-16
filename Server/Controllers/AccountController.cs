using TBAnalisisFinanciero.Server.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Security.AccessControl;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace TBAnalisisFinanciero.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController(
   // UserManager<AppUser> userManager,
   //  IConfiguration configuration,
   // SignInManager<AppUser> signInManager,
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
      return await ForwardRequestAsync("/Account/check-mfa-key", model);
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