using CESistemaLogin.Client.Models;

namespace CESistemaLogin.Client.Services;
public interface IServerCallsService
{
   Task<TokenResponse> LoginUserAsync(LoginRequest requestModel);
}
