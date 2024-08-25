using Microsoft.AspNetCore.StaticFiles;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System.Text;
using CESistemaLogin.ServerApp.Server.Authentication;
using CESistemaLogin.ServerApp.Server.Middlewares;

static void ChecarConfiguracion(WebApplicationBuilder builder)
{
   var apiUrl = builder.Configuration["APIUrl"];
   if (string.IsNullOrEmpty(apiUrl))
   {
      throw new Exception("El valor de ApiUrl ni de ServerUrl pueden ser null o vacío");
   }
}

var builder = WebApplication.CreateBuilder(args);

ChecarConfiguracion(builder);

builder.Services.AddHttpClient("TheApiClient", client =>
{
   string apiUrl = builder.Configuration["APIUrl"]!;
   client.BaseAddress = new Uri(apiUrl);
   client.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.Services.AddHttpClient("TheLocalClient", client =>
{
   string apiUrl = builder.Configuration["LocalAddress"]!;
   client.BaseAddress = new Uri(apiUrl);
   client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddControllers();
builder.Services.AddRazorPages();

builder.Services.AddAuthentication(options =>
{
   // custom scheme defined in .AddPolicyScheme() below
   options.DefaultScheme = "JWT_OR_COOKIE";
   options.DefaultChallengeScheme = "JWT_OR_COOKIE";
   // options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
   options.LoginPath = "/login";
   options.ExpireTimeSpan = TimeSpan.FromDays(1);
})
.AddJwtBearer(options =>
{
   var secret = builder.Configuration["JwtConfig:Secret"];
   var issuer = builder.Configuration["JwtConfig:ValidIssuer"];
   var audience = builder.Configuration["JwtConfig:ValidAudiences"];
   if (secret is null || issuer is null || audience is null)
   {
      throw new ApplicationException("Jwt is not set in the configuration");
   }
   options.SaveToken = true;
   options.RequireHttpsMetadata = false;
   options.TokenValidationParameters = new TokenValidationParameters()
   {
      ValidateIssuer = true,
      ValidateAudience = true,
      ValidateLifetime = true,
      ValidateIssuerSigningKey = true,
      ValidAudience = audience,
      ValidIssuer = issuer,
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
   };
})// this is the key piece!
.AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", options =>
{
   // runs on each request
   options.ForwardDefaultSelector = context =>
   {
      // filter by auth type
      string? authorization = context.Request.Headers[HeaderNames.Authorization];
      if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
         return "Bearer";
      // otherwise always check for cookie auth
      return "Cookies";
   };
});


// Use the policy syntax to add authorization
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireUserNoMFARole", policy => policy.RequireRole(AppRoles.UserNoMFA))
    .AddPolicy("RequireUserRole", policy => policy.RequireRole(AppRoles.User));

const string MyAllowSpecificOrigins = "wasm";
builder.Services.AddCors(options =>
        {
           options.AddPolicy(name: MyAllowSpecificOrigins,
               builder =>
               {
                  builder.SetIsOriginAllowed(origin => new Uri(origin).IsLoopback)
                       .AllowAnyHeader()
                       .AllowAnyMethod();
               });
        });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
   c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
   {
      Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
      Name = "Authorization",
      In = ParameterLocation.Header,
      Type = SecuritySchemeType.ApiKey,
      Scheme = "Bearer"
   });
   c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".yml"] = "application/x-yaml";

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
   app.UseSwagger();
   app.UseSwaggerUI();
   app.UseWebAssemblyDebugging();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();

app.UseStaticFiles(new StaticFileOptions
{
   ContentTypeProvider = provider
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// app.UseMiddleware<AuthMiddleware>();

app.MapControllers();
app.MapRazorPages();

// Mapear a index.html como fallback para rutas no especificadas, excepto /login
app.MapWhen(
    context => !context.Request.Path.StartsWithSegments("/login", StringComparison.OrdinalIgnoreCase),
    builder => builder.UseRouting().UseEndpoints(endpoints =>
    {
        endpoints.MapFallbackToFile("index.html");
    })
);

app.Run();