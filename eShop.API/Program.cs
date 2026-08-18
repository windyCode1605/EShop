using CR.Core.Infrastructure.Persistence.Seeders;
using CR.Core.ApplicationServices.Configs;
using CR.Core.ApplicationServices.OrderModule.Abstracts;
using CR.Core.ApplicationServices.OrderModule.Implements;
using CR.ApplicationBase.Localization;
using CR.WebAPIBase.Filters;
using CR.WebAPIBase.Middlewares;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi.Models;
using OpenIddict.Abstractions;
using DotNetEnv;
using CR.API.Filters;
using CR.Core.ApplicationServices.ProductModule.Abstracts;
using CR.Core.ApplicationServices.Common.ServiceImplementations;
using CR.Core.ApplicationServices.ShipmentModule.Abstracts;
using CR.Core.ApplicationServices.PaymentModule.Implement;
using CR.Core.ApplicationServices.PaymentModule.Abstracts;
using CR.Core.ApplicationServices.ShipmentModule.Implements;
using CR.Core.ApplicationServices.CartModule.Abstracts;
using CR.Core.ApplicationServices.CartModule.Implemts;
using CR.Core.Application.CategoryModule.Abstract;
using CR.Core.Application.CategoryModule.Implements;
using CR.Core.ApplicationServices.AddressModule.Implements;
using CR.Core.ApplicationServices.AddressModule.Abstracts;
using CR.Core.ApplicationServices.AttributeModule.Abstract;
using CR.Core.ApplicationServices.AttributeModule.Implements;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using CR.Core.ApplicationServices.CustomerModule.Abstracts;
using CR.Core.ApplicationServices.CustomerModule.Implements;
using CR.Core.ApplicationServices.SysvarModule.Abstracts;
using CR.Core.ApplicationServices.SysvarModule.Implements;
using CR.Core.ApplicationServices.RoleModule.Abstracts;
using CR.Core.ApplicationServices.RoleModule.Implement;
using CR.Core.ApplicationServices.RoleModule;
using CR.Core.ApplicationServices.DashboardModule.Abstracts;
using CR.Core.ApplicationServices.DashboardModule.Implements;
using CR.Core.ApplicationServices.EmployeeModule.Abstracts;
using CR.Core.ApplicationServices.EmployeeModule.Iplement;


var envPath = Path.Combine(AppContext.BaseDirectory, ".env");
if (!File.Exists(envPath))
{
    envPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".env");
}
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
Console.WriteLine($"[DEBUG] Looking for .env at: {Path.GetFullPath(envPath)}");

if (File.Exists(envPath))
{
    Console.WriteLine($"[DEBUG] Found .env file, loading...");
    Env.Load(envPath);
    Console.WriteLine($"[DEBUG] .env file loaded successfully");
}

Environment.SetEnvironmentVariable("DOTNET_hostBuilder__reloadConfigOnChange", "false");

var builder = WebApplication.CreateBuilder(args);

var renderPort = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(renderPort))
{
    Environment.SetEnvironmentVariable("ASPNET_URLS", $"http://0.0.0.0:{renderPort}");
    Console.WriteLine($"[DEBUG] Running on Cloud (Render) - Port: {renderPort}");
}
else
{
    Console.WriteLine("[DEBUG] Running on Local - Port from launchSettings.json");
}

// Double-safety: clear sources và re-add không có reloadOnChange
builder.Configuration.Sources.Clear();
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables()
    .AddCommandLine(args);

// Override configuration from environment variables
var configMappings = new Dictionary<string, string>
{
    { "CONNECTIONSTRINGS_DEFAULTCONNECTION", "ConnectionStrings:DefaultConnection" },
    { "AUTHENTICATION_GOOGLE_CLIENTID", "Authentication:Google:ClientId" },
    { "AUTHENTICATION_GOOGLE_CLIENTSECRET", "Authentication:Google:ClientSecret" },
    { "JWT_SECRETKEY", "Jwt:SecretKey" },
    { "SMTP_HOST", "Smtp:Host" },
    { "SMTP_PORT", "Smtp:Port" },
    { "SMTP_USERNAME", "Smtp:UserName" },
    { "SMTP_PASSWORD", "Smtp:Password" },
    { "SMTP_FROMEMAIL", "Smtp:FromEmail" },
    { "BREVO_API_KEY", "Brevo:ApiKey" }
};

foreach (var mapping in configMappings)
{
    var envValue = Environment.GetEnvironmentVariable(mapping.Key);
    if (!string.IsNullOrEmpty(envValue))
    {
        builder.Configuration[mapping.Value] = envValue;
    }
}

// Cấu hình SDK Bên thứ 3 (Firebase)
var firebaseKeyPath = builder.Configuration["Firebase:ServiceAccountKeyPath"];
if (!string.IsNullOrEmpty(firebaseKeyPath))
{
    var serviceAccountPath = Path.Combine(builder.Environment.ContentRootPath, firebaseKeyPath);
    if (File.Exists(serviceAccountPath))
    {
        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromFile(serviceAccountPath),
            ProjectId = builder.Configuration["Firebase:projectId"]
        });
    }
}
else
{
    Console.WriteLine("[STARTUP WARNING] Không tìm thấy Firebase:ServiceAccountKeyPath trong biến môi trường! Bỏ qua Firebase.");
}


builder.Logging.ClearProviders();
builder.Logging.AddConsole();
if (builder.Environment.IsDevelopment())
{
    builder.Logging.AddDebug();
}
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));


var mvcBuilder = builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
    options.Filters.Add<PagingValidationFilter>();
});

if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient("BrevoClient", client =>
{
    client.BaseAddress = new Uri("https://api.brevo.com/v3/");
    client.DefaultRequestHeaders.Add("accept", "application/json");
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "CR eShop API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập token vào ô bên dưới theo định dạng: Bearer {access_token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });

    options.CustomSchemaIds(type => type.FullName);
});


builder.Services.Configure<Microsoft.AspNetCore.Builder.ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddDbContext<CoreDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseOpenIddict()
);

// Identity Server (OpenIddict)
builder.Services
    .AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore().UseDbContext<CoreDbContext>();
    })
    .AddServer(options =>
    {
        options.SetAuthorizationEndpointUris("/api/authorization/connect/authorize")
               .SetTokenEndpointUris("/connect/token");

        options.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange();
        options.AllowRefreshTokenFlow();
        options.AllowPasswordFlow();
        options.AllowClientCredentialsFlow();

        options.RegisterScopes(
            OpenIddictConstants.Scopes.Email,
            OpenIddictConstants.Scopes.Profile,
            OpenIddictConstants.Scopes.Roles,
            "api"
        );

        options.AddDevelopmentEncryptionCertificate().AddDevelopmentSigningCertificate();
        options.DisableAccessTokenEncryption();

        var aspNetCore = options.UseAspNetCore()
            .EnableAuthorizationEndpointPassthrough()
            .EnableTokenEndpointPassthrough();

        if (builder.Environment.IsDevelopment())
        {
            aspNetCore.DisableTransportSecurityRequirement();
        }
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });


builder.Services.AddCors(options =>
{
    options.AddPolicy("WebClient", policy =>
    {
        policy.SetIsOriginAllowed(origin => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/auth/login";
    options.AccessDeniedPath = "/auth/denied";
    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        }
        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
});

// Google OAuth Check
var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
if (!string.IsNullOrEmpty(googleClientId) &&
    googleClientId != "your-google-client-id" &&
    !string.IsNullOrEmpty(googleClientSecret))
{
    Console.WriteLine("[CONFIG] Google OAuth: Enabled");
}
else
{
    Console.WriteLine("[CONFIG] Google OAuth: Disabled (ClientId not configured)");
}

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiUser", policy => policy.RequireAuthenticatedUser());
});

// Distributed Caching (Redis / MemoryCache)
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrEmpty(redisConnectionString))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnectionString;
        options.InstanceName = "eShop_";
    });
}
else
{
    builder.Services.AddDistributedMemoryCache();
}

// Infrastructure & Localization
builder.Services.AddSingleton<IMapErrorCode, MapErrorCode>();
builder.Services.AddSingleton<LocalizationBase>();
builder.Services.AddSingleton<ILocalization>(sp => sp.GetRequiredService<LocalizationBase>());

// Authorization & Permission System
builder.Services.AddScoped<CR.Core.ApplicationServices.AuthenticationModule.Abstracts.IPermissionCacheService, CR.Core.ApplicationServices.AuthenticationModule.Implements.PermissionCacheService>();
builder.Services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationPolicyProvider, CR.Core.API.Authorization.PermissionPolicyProvider>();
builder.Services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, CR.Core.API.Authorization.PermissionAuthorizationHandler>();

// Core Application Domain Services
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IShipmentService, ShipmentService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IAttributeService, AttributeService>();
builder.Services.AddScoped<IAttributeValueService, AttributeValueService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IAdminCustomerService, AdminCustomerService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ISysvarService, SysVarService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

builder.Services.AddSingleton<eShop.API.Services.Shared.FirebaseStorageService>();
builder.Services.AddSingleton<eShop.API.Services.Shared.FirebaseNotificationService>();

builder.Services.AddApplicationServices();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var localization = scope.ServiceProvider.GetRequiredService<CR.ApplicationBase.Localization.LocalizationBase>();
    localization.LoadDictionary("eShop.API.Resources");

    var db = scope.ServiceProvider.GetRequiredService<CoreDbContext>();

    try
    {
        Console.WriteLine("[STARTUP] Running database migration...");
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(25));
        await db.Database.MigrateAsync(cts.Token);
        Console.WriteLine("[STARTUP] Migration completed.");

        await DataSeeder.SeedAsync(db);
        Console.WriteLine("[STARTUP] Data seeding completed.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[STARTUP WARNING] Migration/Seed skipped: {ex.Message}");
        Console.WriteLine("[STARTUP] App will continue without migration (DB schema already up-to-date).");
    }

    try
    {
        var applicationManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        if (await applicationManager.FindByClientIdAsync("client-web") == null)
        {
            var clientSecret = builder.Configuration["OpenIddict:ClientWeb:Secret"] 
                ?? throw new InvalidOperationException("Missing OpenIddict Client Secret");

            var descriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = "client-web",
                ClientSecret = clientSecret,
                DisplayName = "Web Client",
            };

            descriptor.RedirectUris.Add(new Uri("http://localhost:4200/callback"));
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Authorization);
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode);
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.RefreshToken);
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.Password);
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.ClientCredentials);
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Code);
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.Scopes.Email);
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.Scopes.Profile);
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.Scopes.Roles);
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.Prefixes.Scope + "api");

            await applicationManager.CreateAsync(descriptor);
            Console.WriteLine("[STARTUP] OpenIddict client-web created.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[STARTUP WARNING] OpenIddict seeding skipped: {ex.Message}");
    }
}

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseForwardedHeaders();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
var webRootPath = app.Environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
if (!Directory.Exists(webRootPath)) Directory.CreateDirectory(webRootPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(webRootPath),
    RequestPath = ""
});

app.UseRouting();
app.UseCors("WebClient");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var finalPort = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(finalPort))
{
    app.Run($"http://0.0.0.0:{finalPort}");
}
else
{
    app.Run();
}