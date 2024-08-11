using TBAnalisisFinanciero.Client.Models;

namespace TBAnalisisFinanciero.Client.Services;
public interface IAuthenticationService
{
   Task<TokenResponse> LoginUserAsync(LoginRequest requestModel);
}
