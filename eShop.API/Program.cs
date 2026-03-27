using CR.Core.ApplicationServices.Configs;
using CR.Core.Domain.Category;
using CR.WebAPIBase.Filters;
using CR.WebAPIBase.Middlewares;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());

// DataBase
builder.Services.AddDbContext<CoreDbContext>(option =>

    option.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
));
// 2.
// DI (Dependency Injection)
// builder.Services.AddScoped<IMyService, MyService>();
builder.Services.AddApplicationServices(); // Extension method để đăng ký các service của tầng ApplicationServices.

// Middleware
// 5. Middleware từ CR.WebAPIBase
builder.Services.AddControllers(opt =>
    opt.Filters.Add<ValidationFilter>()
);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
    await db.Database.ExecuteSqlRawAsync(@"
IF OBJECT_ID(N'[core].[Products]', N'U') IS NOT NULL
AND COL_LENGTH('core.Products', 'IsDeleted') IS NULL
BEGIN
    ALTER TABLE [core].[Products]
    ADD [IsDeleted] bit NOT NULL
    CONSTRAINT [DF_Products_IsDeleted] DEFAULT (0);
END");

    // Seed test categories if table is empty
    var categoryCount = await db.Set<Categories>().CountAsync();
    if (categoryCount == 0)
    {
        var testCategories = new[]
        {
            new Categories { Name = "Electronics", Slug = "electronics" },
            new Categories { Name = "Clothing", Slug = "clothing" },
            new Categories { Name = "Books", Slug = "books" },
            new Categories { Name = "Home & Garden", Slug = "home-garden" }
        };
        db.Set<Categories>().AddRange(testCategories);
        await db.SaveChangesAsync();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();  // Từ CR.WebAPIBase
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseDeveloperExceptionPage(); // Hiển thị chi tiết lỗi trong môi trường phát triển (chỉ nên dùng trong môi trường phát triển)
app.Run();