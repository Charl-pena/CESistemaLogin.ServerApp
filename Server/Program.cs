using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TBAnalisisFinanciero.Server.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// builder.Services.AddDbContext<AppDbContext>();

// builder.Services.AddIdentityCore<AppUser>()
    // .AddRoles<IdentityRole>()
    // .AddSignInManager()
    // .AddEntityFrameworkStores<AppDbContext>()
    // .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
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
});

// builder.Services.Configure<IdentityOptions>(options =>
// {
//     // Password settings
//     options.Password.RequireDigit = true;
//     options.Password.RequireLowercase = true;
//     options.Password.RequireNonAlphanumeric = true;
//     options.Password.RequireUppercase = true;
//     options.Password.RequiredLength = 8;
//     options.Password.RequiredUniqueChars = 1;
//     // User settings
//     options.User.RequireUniqueEmail = true;
//     // Lockout settings
//     options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
//     options.Lockout.MaxFailedAccessAttempts = 3;
//     options.Lockout.AllowedForNewUsers = true;
// });

// Use the policy syntax to add authorization
builder.Services.AddAuthorizationBuilder()
   // Use the policy syntax to add authorization
    .AddPolicy("RequireAdministratorRole", policy => policy.RequireRole(AppRoles.Administrator))
    .AddPolicy("RequireVipUserRole", policy => policy.RequireRole(AppRoles.VipUser))
    .AddPolicy("RequireUserRole", policy => policy.RequireRole(AppRoles.User));

const string MyAllowSpecificOrigins = "Cors-Settings";
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseWebAssemblyDebugging();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();

var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".yml"] = "application/x-yaml";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider
});

app.UseRouting();

app.Use(async (context, next) =>
{
    // Verificar si la solicitud tiene un token en la URL
    var token = context.Request.Query["token"].ToString();
    if (!string.IsNullOrEmpty(token))
    {
        // Si se encuentra el token, añadirlo al encabezado Authorization
        context.Request.Headers.Append("Authorization", $"Bearer {token}");
    }

    // Llamar al siguiente middleware
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

// using (var serviceScope = app.Services.CreateScope())
// {
//     var services = serviceScope.ServiceProvider;
//     // Ensure the database is created.
//     var dbContext = services.GetRequiredService<AppDbContext>();
//     //dbContext.Database.EnsureDeleted();
//     dbContext.Database.EnsureCreated();

//     var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
//     if (!await roleManager.RoleExistsAsync(AppRoles.User))
//     {
//         await roleManager.CreateAsync(new IdentityRole(AppRoles.User));
//     }
//     if (!await roleManager.RoleExistsAsync(AppRoles.VipUser))
//     {
//         await roleManager.CreateAsync(new IdentityRole(AppRoles.VipUser));
//     }
//     if (!await roleManager.RoleExistsAsync(AppRoles.Administrator))
//     {
//         await roleManager.CreateAsync(new IdentityRole(AppRoles.Administrator));
//     }
// }

app.MapRazorPages();
app.MapControllers();

app.Use(async (context, next) =>
{
     // Revisa si el usuario está autenticado
    if (context.User.Identity?.IsAuthenticated ?? false)
    {
        // Si está autenticado, sirve el archivo index.html
        await next();
    }
    else
    {
        // Si no está autenticado, retorna un 401 o redirige a una página de login
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    }
});

app.MapFallbackToFile("index.html");

app.Run();