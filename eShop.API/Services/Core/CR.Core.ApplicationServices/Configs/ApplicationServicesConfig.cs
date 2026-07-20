using CR.Core.ApplicationServices.AuthenticationModule.Abstracts;
using CR.Core.ApplicationServices.AuthenticationModule.Implements;
using CR.Core.ApplicationServices.Common.ServiceImplementations;
using CR.Core.ApplicationServices.CustomerModule.Abstracts;
using CR.Core.ApplicationServices.CustomerModule.Implements;
using CR.Core.ApplicationServices.OtpModule.Abstracts;
using CR.Core.ApplicationServices.OtpModule.Implements;
using CR.Core.ApplicationServices.ProductModule.Abstracts;

namespace CR.Core.ApplicationServices.Configs;

// Cấu hình Dependency Injection cho tầng ApplicationServices.
// File này gom các đăng ký service để Program.cs chỉ cần gọi 1 extension method.
public static class ApplicationServicesConfig
{
    // Đăng ký các service của tầng Application vào IServiceCollection.
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Authentication & User services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserAuthenticationService, UserAuthorizationService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<INotificationTokenService, NotificationTokenService>();
        services.AddScoped<IOtpService, OtpService>();

        // Product services
        services.AddScoped<IProductService, ProductService>();

        // Customer services
        services.AddScoped<ICustomerService, CustomerService>();

        // Quét assembly hiện tại để nạp các AutoMapper Profile.
        services.AddAutoMapper(cfg => { }, typeof(ApplicationServicesConfig).Assembly);

        // Trả về service collection để có thể chain với các cấu hình khác.
        return services;

    }
}
