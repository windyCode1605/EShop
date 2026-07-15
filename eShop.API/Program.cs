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


// ===============================================
// 0. ENVIRONMENT & CONFIGURATION LOAD
// ===============================================
var envPath = Path.Combine(AppContext.BaseDirectory, ".env");
if (!File.Exists(envPath))
{
    envPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".env");
}

Console.WriteLine($"[DEBUG] Looking for .env at: {Path.GetFullPath(envPath)}");

if (File.Exists(envPath))
{
    Console.WriteLine($"[DEBUG] Found .env file, loading...");
    Env.Load(envPath);
    Console.WriteLine($"[DEBUG] .env file loaded successfully");
}

var builder = WebApplication.CreateBuilder(args);

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
    { "SMTP_FROMEMAIL", "Smtp:FromEmail" }
};

foreach (var mapping in configMappings)
{
    var envValue = Environment.GetEnvironmentVariable(mapping.Key);
    if (!string.IsNullOrEmpty(envValue))
    {
        builder.Configuration[mapping.Value] = envValue;
    }
}

// ===============================================
// 1. LOGGING SETUP
// ===============================================
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
if (builder.Environment.IsDevelopment())
{
    builder.Logging.AddDebug();
}
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

// ===============================================
// 2. CORE API SERVICES & CONTROLLERS
// ===============================================
// Đã gộp 2 lần AddControllers/AddControllersWithViews thành 1 khối duy nhất
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ValidationFilter>();
    options.Filters.Add<PagingValidationFilter>();
})
.AddRazorRuntimeCompilation();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

// ===============================================
// 3. SWAGGER CONFIGURATION
// ===============================================
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
});

// ===============================================
// 4. DATABASE & OPENIDDICT CONFIGURATION
// ===============================================
builder.Services.AddDbContext<CoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseOpenIddict()
);

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

// ===============================================
// 5. AUTHENTICATION, AUTHORIZATION & CORS
// ===============================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("WebClient", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
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
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiUser", policy => policy.RequireAuthenticatedUser());
});

// Configure Distributed Cache
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
    // Fallback if Redis is not configured
    builder.Services.AddDistributedMemoryCache();
}

// ===============================================
// 6. DEPENDENCY INJECTION (DI) 
// ===============================================
// Infrastructure / Common
builder.Services.AddSingleton<IMapErrorCode, MapErrorCode>();
builder.Services.AddSingleton<LocalizationBase>();
builder.Services.AddSingleton<ILocalization>(sp => sp.GetRequiredService<LocalizationBase>());

// Authorization Cache
builder.Services.AddScoped<CR.Core.ApplicationServices.AuthenticationModule.Abstracts.IPermissionCacheService, CR.Core.ApplicationServices.AuthenticationModule.Implements.PermissionCacheService>();
builder.Services.AddScoped<CR.Core.ApplicationServices.AuthenticationModule.Abstracts.IRoleService, CR.Core.ApplicationServices.AuthenticationModule.Implements.RoleService>();
builder.Services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationPolicyProvider, CR.Core.API.Authorization.PermissionPolicyProvider>();
builder.Services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, CR.Core.API.Authorization.PermissionAuthorizationHandler>();

// Application Services (Giải quyết lỗi IOrderService)
// BẠN KHAI BÁO CÁC SERVICE CÒN THIẾU TẠI ĐÂY
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IShipmentService, ShipmentService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IAttributeService, AttributeService>();
builder.Services.AddScoped<IAttributeValueService, AttributeValueService>();

// builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Nơi bạn đã đóng gói các DI khác thông qua Extension Method
builder.Services.AddApplicationServices();

// ===============================================
// 7. BUILD APP & DATABASE SEEDING
// ===============================================
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
    await db.Database.MigrateAsync();
    // Tự động Seed Data mỗi lần chạy
    await DataSeeder.SeedAsync(db);

    var applicationManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

    if (await applicationManager.FindByClientIdAsync("client-web") == null)
    {
        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = "client-web",
            ClientSecret = "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",
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
    }
}

// ===============================================
// 8. HTTP REQUEST PIPELINE (THỨ TỰ CHUẨN)
// ===============================================
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 1. Exception Handling
app.UseMiddleware<GlobalExceptionMiddleware>();

// 2. HTTPS & Static Files
app.UseHttpsRedirection();
app.UseStaticFiles();

// 3. Routing & CORS
app.UseRouting();
app.UseCors("WebClient");

// 4. Authentication & Authorization (Phải nằm giữa Routing và Endpoints)
app.UseAuthentication();
app.UseAuthorization();

// 5. Endpoints
app.MapControllers();

app.Run();