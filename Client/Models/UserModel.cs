using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace TBAnalisisFinanciero.Client.Models;

public class UserModel
{
   public string unique_name { get; set; } = string.Empty; 
   public ClaimsPrincipal ToClaimsPrincipal() => new(new ClaimsIdentity
   (new Claim[]
    {
        new (nameof(unique_name), unique_name),
    }, "Bearer"));

   public static UserModel FromClaimsPrincipal(ClaimsPrincipal principal) => new()
   {
      unique_name = principal.FindFirst(nameof(unique_name))?.Value ?? ""
   };
}
