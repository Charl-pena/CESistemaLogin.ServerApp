using TBAnalisisFinanciero.Server.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TBAnalisisFinanciero.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController(
   UserManager<AppUser> userManager,
   IConfiguration configuration,
   SignInManager<AppUser> signInManager)
   : ControllerBase
{
   [HttpPost("register")]
   public Task<IActionResult> Register([FromBody] AddOrUpdateAppUserModel model)
   {
      return RegisterUserWithRole(model, AppRoles.User);
   }

   [HttpPost("register-admin")]
   public Task<IActionResult> RegisterAdmin([FromBody] AddOrUpdateAppUserModel model)
   {
      return RegisterUserWithRole(model, AppRoles.Administrator);
   }

   [HttpPost("register-vip")]
   public Task<IActionResult> RegisterVip([FromBody] AddOrUpdateAppUserModel model)
   {
      return RegisterUserWithRole(model, AppRoles.VipUser);
   }

   // Create an action to register a new user
   private async Task<IActionResult> RegisterUserWithRole(AddOrUpdateAppUserModel model, string roleName)
   {
      // Check if the model is valid
      if (ModelState.IsValid)
      {
         var existedUser = await userManager.FindByNameAsync(model.UserName);
         if (existedUser != null)
         {
            ModelState.AddModelError("", "User name is already taken");
            return BadRequest(ModelState);
         }
         // Create a new user object
         var user = new AppUser()
         {
            UserName = model.UserName,
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString()
         };
         // Try to save the user
         var userResult = await userManager.CreateAsync(user, model.Password);
         // Add the user to the role
         var roleResult = await userManager.AddToRoleAsync(user, roleName);
         // If the user is successfully created, return Ok
         if (userResult.Succeeded && roleResult.Succeeded)
         {
            var createdUser = await userManager.FindByNameAsync(model.UserName);
            var token = GenerateToken(createdUser!);
            return Ok(new { token });
         }
         // If there are any errors, add them to the ModelState object
         // and return the error to the client
         foreach (var error in userResult.Errors)
         {
            ModelState.AddModelError("", error.Description);
         }

         foreach (var error in roleResult.Errors)
         {
            ModelState.AddModelError("", error.Description);
         }
      }
      // If we got this far, something failed, redisplay form
      return BadRequest(ModelState);
   }

   // Create a Login action to validate the user credentials and generate the JWT token
   [HttpPost("login")]
   public async Task<IActionResult> Login([FromBody] LoginModel model)
   {
      // Get the secret in the configuration

      // Check if the model is valid
      if (ModelState.IsValid)
      {
         var user = await userManager.FindByNameAsync(model.UserName) ?? await userManager.FindByEmailAsync(model.UserName);
         if (user != null)
         {
            var result =
               await signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: true);
            if (result.Succeeded)
            {
               var token = await GenerateToken(user);
               return Ok(new { token });
            }
         }
         // If the user is not found, display an error message
         ModelState.AddModelError("", "Invalid username or password");
      }
      return BadRequest(ModelState);
   }

   private async Task<string?> GenerateToken(AppUser user )
   {
      var secret = configuration["JwtConfig:Secret"];
      var issuer = configuration["JwtConfig:ValidIssuer"];
      var audience = configuration["JwtConfig:ValidAudiences"];
      if (secret is null || issuer is null || audience is null)
      {
         throw new ApplicationException("Jwt is not set in the configuration");
      }
      var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
      var tokenHandler = new JwtSecurityTokenHandler();

      var userRoles = await userManager.GetRolesAsync(user);
      var userName = await userManager.GetUserNameAsync(user);
      var userEmail = await userManager.GetEmailAsync(user);

      if(userName ==null || userEmail == null){
         throw new ApplicationException("userName no puede ser Null. | Revisa la operacion GetUserNameAsync()");
      }

      var claims = new List<Claim>
        {
            new(ClaimTypes.Name, userEmail),
            new(ClaimTypes.GivenName, userName),
        };
      claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));
      var tokenDescriptor = new SecurityTokenDescriptor
      {
         Subject = new ClaimsIdentity(claims),
         Expires = DateTime.UtcNow.AddHours(1),
         Issuer = issuer,
         Audience = audience,
         SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature)
      };

      var token = tokenHandler.CreateToken(tokenDescriptor);

      // var jwtToken = new JwtSecurityToken(
      //    issuer: issuer,
      //    audience: audience,
      //    claims: new[]{
      //        new Claim(ClaimTypes.Name, userName)
      //    },
      //    expires: DateTime.UtcNow.AddHours(1),
      //    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature)
      // );
      var tokenString = tokenHandler.WriteToken(token);
      return tokenString;
   }
}