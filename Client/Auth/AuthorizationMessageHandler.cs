using Blazored.LocalStorage;

namespace CESistemaLogin.Client.Auth;

public class AuthorizationMessageHandler : DelegatingHandler
{
	private const string LocalStorageKey = "o8yo82q43rtbuiibeWQAFY8";
	private readonly ILocalStorageService _localStorage;

	public AuthorizationMessageHandler(ILocalStorageService localStorage)
	{
		_localStorage = localStorage;
	}

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		if(   !await _localStorage.ContainKeyAsync("000001", cancellationToken))
		{
			if (await _localStorage.ContainKeyAsync(LocalStorageKey, cancellationToken))
			{
				var token = await _localStorage.GetItemAsync<string>(LocalStorageKey, cancellationToken);
				request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
			}
		}
		
		return await base.SendAsync(request, cancellationToken);
	}
}