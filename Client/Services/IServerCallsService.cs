using CESistemaLogin.ServerApp.Client.Models;

namespace CESistemaLogin.ServerApp.Client.Services;
public interface IServerCallsService
{
   Task<TokenResponse> LoginUserAsync(LoginRequest requestModel);
}
