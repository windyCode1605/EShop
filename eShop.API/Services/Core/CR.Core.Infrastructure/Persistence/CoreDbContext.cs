using System.Security.Cryptography.X509Certificates;
using CR.Core.Domain.AuthToken;
using CR.Core.Domain.Carts;
using CR.Core.Domain.Catalog;
using CatalogAttribute = CR.Core.Domain.Catalog.Attribute;
using CR.Core.Domain.Orders;
using CR.Core.Domain.SysVar;
using CR.Core.Domain.User;
using CR.Core.Domain.Payment;
using CR.Core.Domain.Address;
using CR.Core.Domain.Review;
using CR.Core.Domain.Coupons;
using CR.Core.Domain.Logistics;
using CR.Core.Domain.Opts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;         // IHttpContextAccessor
using Microsoft.Extensions.Logging;       // ILogger<T>
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion; // GetService<T>()

public class CoreDbContext : ApplicationDbContext<Users>
{
    public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options) { }

    // === CATALOG ===
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<CatalogAttribute> Attributes { get; set; }
    public DbSet<AttributeValue> AttributeValues { get; set; }
    public DbSet<ProductVariantAttribute> ProductVariantAttributes { get; set; }
    public DbSet<ProductAttribute> ProductAttributes { get; set; }
    public DbSet<CategoryAttribute> CategoryAttributes { get; set; }
    // === CART & ORDER ===
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderRefund> OrderRefunds { get; set; }

    // === PAYMENT & LOGISTICS ===
    public DbSet<Payments> Payments { get; set; }
    public DbSet<Shipment> Shipments { get; set; }

    // === PROMOTION ===
    public DbSet<Coupons> Coupons { get; set; }
    public DbSet<CouponUsage> CouponUsages { get; set; }

    // === USER & AUTHENTICATION ===
    public DbSet<Users> Users { get; set; }
    public DbSet<NotificationToken> NotificationTokens { get; set; }
    public DbSet<AuthOtp> AuthOtps { get; set; }
    public DbSet<SendOtp> SendOtps { get; set; }

    // === USER PROFILE & PERMISSION ===
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }

    // === REVIEW & ADDRESS ===
    public DbSet<Reviews> Reviews { get; set; }
    public DbSet<Addresses> Addresses { get; set; }

    // === SYSTEM ===
    public DbSet<SysVar> SysVars { get; set; }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);
        mb.ApplyConfigurationsFromAssembly(typeof(CoreDbContext).Assembly);

        // Tìm tất cả các properties kiểu DateTime và gắn ValueConverter
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? v.Value.ToUniversalTime() : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

        foreach (var entityType in mb.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(nullableDateTimeConverter);
                }
            }
        }
    }
}
