using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace CESistemaLogin.Client.Auth;

public class UserModel
{
   public string Unique_name { get; set; } = string.Empty; 
   public ClaimsPrincipal ToClaimsPrincipal() => new(new ClaimsIdentity
   (new Claim[]
    {
        new (nameof(Unique_name), Unique_name),
    }, "Bearer"));

   public static UserModel FromClaimsPrincipal(ClaimsPrincipal principal) => new()
   {
      Unique_name = principal.FindFirst(nameof(Unique_name))?.Value ?? ""
   };
}
