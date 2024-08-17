using Blazored.LocalStorage;

namespace TBAnalisisFinanciero.Client.Auth;

public class AuthorizationMessageHandler : DelegatingHandler
{

	private readonly ILocalStorageService _localStorage;

	public AuthorizationMessageHandler(ILocalStorageService localStorage)
	{
		_localStorage = localStorage;
	}

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		if (await _localStorage.ContainKeyAsync("o8yo82q43rtbuiibeWQAFY8GFWEIGUO7G8FLKBJ", cancellationToken))
		{
			var token = await _localStorage.GetItemAsync<string>("o8yo82q43rtbuiibeWQAFY8GFWEIGUO7G8FLKBJ", cancellationToken);
			request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
		}
		
		return await base.SendAsync(request, cancellationToken);
	}
}