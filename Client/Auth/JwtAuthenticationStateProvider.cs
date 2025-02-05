using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CESistemaLogin.ServerApp.Client.Models;

namespace CESistemaLogin.ServerApp.Client.Auth;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
	private const string LocalStorageKey = "o8yo82q43rtbuiibeWQAFY8";
	private readonly ILocalStorageService _localStorage;
	
	public UsuarioActual CurrentUser { get; private set; } = new();

	public JwtAuthenticationStateProvider(ILocalStorageService storage) 
	{
		_localStorage = storage;
	}

   public async override Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		var tokenHandler = new JwtSecurityTokenHandler()
		{
			//Desactivar el mappeo de claims con valores por defecto definidos por la libreria de ASP.NET
         MapInboundClaims = false
      };

		var currentUsertoken = await GetCurrentUserAsync();

		if(currentUsertoken != null && tokenHandler.CanReadToken(currentUsertoken))
      {
			var token = tokenHandler.ReadJwtToken(currentUsertoken);
			var claims = token.Claims.ToList();
		
			var identity = new ClaimsIdentity(claims, "Bearer", JwtRegisteredClaimNames.Sub, "role");
			var user = new ClaimsPrincipal(identity);
			var authState = new AuthenticationState(user);
			if (user != null)
			{
				CurrentUser = UsuarioActual.FromClaimsPrincipal(user);
			}
			return authState;
      }
		else
		{
			var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity()); // No claims or authentication scheme provided
			return new AuthenticationState(anonymousUser);
		}
	}

	public async Task SetCurrentUserAsync(string currentUsertoken, bool isDummy = false)
   { 	
		if(isDummy)
		{
			await _localStorage.SetItemAsync("000001", true);
		}
      await _localStorage.SetItemAsStringAsync(LocalStorageKey, currentUsertoken);
      NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
   }
	public async Task LogoutCurrentUserAsync()
   { 	
		if( await _localStorage.ContainKeyAsync("o8yo82q43rtbuiibeWQAFY8")){
			await _localStorage.RemoveItemAsync("o8yo82q43rtbuiibeWQAFY8");
		}
		if( await _localStorage.ContainKeyAsync("000001")){
			await _localStorage.RemoveItemAsync("000001");
		}
      NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
   }

   private async Task<string?> GetCurrentUserAsync() => await _localStorage.GetItemAsStringAsync(LocalStorageKey);
}
