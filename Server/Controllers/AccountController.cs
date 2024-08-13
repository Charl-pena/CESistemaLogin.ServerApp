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
   public async Task<IActionResult> APIMfa([FromBody] UserNameModel model)
   {
      var uri = "/Account/mfa"; // Ruta específica sin la BaseAddress
      // Check if the model is valid
      if (ModelState.IsValid)
      {
         var headerAuth =
            Request.Headers.Authorization[0]?.Split(" ");
         if(headerAuth != null){
            try
            {
               // Serialize the model to JSON
               var jsonContent = new StringContent(
                  JsonSerializer.Serialize(model),
                  Encoding.UTF8,
                  "application/json"
               );
               _httpClient.DefaultRequestHeaders.Authorization = 
                     new AuthenticationHeaderValue(headerAuth[0], headerAuth[1]);
               // Send the POST request to the destination API
               var response = await _httpClient.PostAsync(uri, jsonContent);

               // Check if the response was successful
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
      return BadRequest(ModelState);
   }

   // [HttpPost("login")]
   // public async Task<IActionResult> Login([FromBody] LoginModel model)
   // {
   //    // Check if the model is valid
   //    if (ModelState.IsValid)
   //    {
   //       var user = await userManager.FindByNameAsync(model.UserName) ?? await userManager.FindByEmailAsync(model.UserName);
   //       if (user != null)
   //       {
   //          var result =
   //             await signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: true);
   //          if (result.Succeeded)
   //          {
   //             return Ok();
   //          }
   //       }
   //       // If the user is not found, display an error message
   //       ModelState.AddModelError("", "Invalid username or password");
   //    }
   //    return BadRequest(ModelState);
   // }
}