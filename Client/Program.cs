using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;

using Blazored.LocalStorage;
using CEBlazorBulma.Configuration;
using TBAnalisisFinanciero.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<AuthorizationMessageHandler>();
builder.Services.AddHttpClient("TheServerAPI", client => client.BaseAddress =new Uri(builder.HostEnvironment.BaseAddress))
	.AddHttpMessageHandler<AuthorizationMessageHandler>();
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("TheServerAPI"));

builder.Services.AddBlazoredLocalStorage();

// Registrar los servicios de CEBlazorBulma utilizando el cliente "TheServerAPI"
builder.Services.AddCENavBarServices("TheServerAPI");
builder.Services.AddCEYamlService(sp => YamlDeserializerFactory.CreateDeserializer(), "TheServerAPI");

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();

await builder.Build().RunAsync();