using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TBAnalisisFinanciero.Client;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
	private readonly ILocalStorageService _storage;
	public JwtAuthenticationStateProvider(ILocalStorageService storage) 
	{
		_storage = storage;
	}

	public async override Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		if (await _storage.ContainKeyAsync("o8yo82q43rtbuiibeWQAFY8GFWEIGUO7G8FLKBJ"))
		{
			// Read and parse the token 
			var tokenAsString = await _storage.GetItemAsync<string>("o8yo82q43rtbuiibeWQAFY8GFWEIGUO7G8FLKBJ");
			var tokenHandler = new JwtSecurityTokenHandler()
			{
				//Desactivar el mappeo de claims con valores por defecto definidos por la libreria de ASP.NET
            MapInboundClaims = false
      	};
			//Hay que checar que sea un token bien formado
			if(tokenHandler.CanReadToken(tokenAsString))
			{
				var token = tokenHandler.ReadJwtToken(tokenAsString);
				var claims = token.Claims.ToList();
				
			
				var identity = new ClaimsIdentity(claims, "jwt", JwtRegisteredClaimNames.Sub, "role");
				var user = new ClaimsPrincipal(identity);
				var authState = new AuthenticationState(user);
				
				NotifyAuthenticationStateChanged(Task.FromResult(authState));

				return authState;
			}
		}
		var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity()); // No claims or authentication scheme provided
		var anonymousAuthState = new AuthenticationState(anonymousUser);
		NotifyAuthenticationStateChanged(Task.FromResult(anonymousAuthState));
		return anonymousAuthState;
	}
}
