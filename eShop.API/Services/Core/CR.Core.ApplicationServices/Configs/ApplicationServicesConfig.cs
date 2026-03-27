using CR.Core.ApplicationServices.Common.ServiceImplementations;
using CR.Core.ApplicationServices.ProductModule.Abstracts;

namespace CR.Core.ApplicationServices.Configs;

// Cấu hình Dependency Injection cho tầng ApplicationServices.
// File này gom các đăng ký service để Program.cs chỉ cần gọi 1 extension method.
public static class ApplicationServicesConfig
{
    // Đăng ký các service của tầng Application vào IServiceCollection.
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Mỗi request HTTP nhận một instance ProductService mới.
        services.AddScoped<IProductService, ProductService>();

        // Quét assembly hiện tại để nạp các AutoMapper Profile.
        services.AddAutoMapper(cfg => { }, typeof(ApplicationServicesConfig).Assembly);

        // Trả về service collection để có thể chain với các cấu hình khác.
        return services;

    }
}