using CR.Core.ApplicationServices.Configs;
using CR.Core.Domain.Catalog;
using CR.ApplicationBase.Localization;
using CR.WebAPIBase.Filters;
using CR.WebAPIBase.Middlewares;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models; // Đã thêm thư viện chuẩn cho Swagger
using OpenIddict.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// ===============================================
// 1. BASIC SERVICES & SWAGGER
// ===============================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "CR eShop API", Version = "v1" });

    // Khai báo nút Authorize nhập JWT Token
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập token vào ô bên dưới theo định dạng: Bearer {access_token}"
    });

    // Áp dụng ổ khóa (Security Requirement) cho tất cả API
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IMapErrorCode, MapErrorCode>();
builder.Services.AddSingleton<LocalizationBase>();
builder.Services.AddSingleton<ILocalization>(sp => sp.GetRequiredService<LocalizationBase>());

builder.Logging.ClearProviders();

if (builder.Environment.IsDevelopment())
{
    builder.Logging.AddConsole();
    builder.Logging.AddDebug();
}
else
{
    builder.Logging.AddConsole();
}

builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

// ===============================================
// 2. CORS POLICY
// ===============================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("WebClient", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials(); // Critical for OAuth flows
    });
});

// ===============================================
// 3. AUTHENTICATION & AUTHORIZATION
// ===============================================
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/auth/login";
    options.AccessDeniedPath = "/auth/denied";

    // QUAN TRỌNG: tránh redirect ra trang HTML đăng nhập khi gọi API bị lỗi 401
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
    options.AddPolicy("ApiUser", policy =>
    {
        policy.RequireAuthenticatedUser();
    });
});

// ===============================================
// 4. DATABASE CONFIGURATION
// ===============================================
builder.Services.AddDbContext<CoreDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
    .UseOpenIddict()
);

// ===============================================
// 5. OPENIDDICT CONFIGURATION (OAUTH2 SERVER)
// ===============================================
builder.Services
    .AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
               .UseDbContext<CoreDbContext>();
    })
    .AddServer(options =>
    {
        options.SetAuthorizationEndpointUris("/api/authorization/connect/authorize")
               .SetTokenEndpointUris("/connect/token");

        // Các luồng cấp Token được phép
        options.AllowAuthorizationCodeFlow()
               .RequireProofKeyForCodeExchange();
        options.AllowRefreshTokenFlow();
        options.AllowPasswordFlow();
        options.AllowClientCredentialsFlow();

        options.RegisterScopes(
            OpenIddictConstants.Scopes.Email,
            OpenIddictConstants.Scopes.Profile,
            OpenIddictConstants.Scopes.Roles,
            "api"
        );

        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();

        var aspNetCore = options.UseAspNetCore()
            .EnableAuthorizationEndpointPassthrough()
            .EnableTokenEndpointPassthrough();

        if (builder.Environment.IsDevelopment())
        {
            aspNetCore.DisableTransportSecurityRequirement(); // Tắt bắt buộc HTTPS khi dev local
        }
    });

// ===============================================
// 6. DEPENDENCY INJECTION & MVC
// ===============================================
// AddApplicationServices() tự chứa cấu hình AutoMapper bên trong
builder.Services.AddApplicationServices(); 

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ValidationFilter>();
})
.AddRazorRuntimeCompilation();

// ===============================================
// 7. BUILD APP & DATABASE INITIALIZATION
// ===============================================
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
    // Use EnsureCreatedAsync instead of MigrateAsync to create schema without migrations
    // This bypasses the pending model changes validation issue
    await db.Database.EnsureCreatedAsync();

    // Seed OpenIddict Application (Tạo Client mặc định cho Web Angular)
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
// 8. MIDDLEWARE PIPELINE (THỨ TỰ CỰC KỲ QUAN TRỌNG)
// ===============================================
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(); // Nút Authorize xanh lá sẽ xuất hiện ở đây
}

app.UseMiddleware<GlobalExceptionMiddleware>(); // Bắt lỗi tổng
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("WebClient");

// AuthN luôn phải đứng trước AuthZ
app.UseAuthentication(); 
app.UseAuthorization();  

app.MapControllers();

app.Run();