using System.Security.Cryptography.X509Certificates;
using CR.Core.Domain.Order;
using CR.Core.Domain.Product;
using CR.Core.Domain.User;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;

public class CoreDbContext : ApplicationDbContext<Users>
{
    public CoreDbContext(DbContextOptions options) : base(options){}
    public DbSet<Products> Products { get; set; }
    public DbSet<Orders> Orders { get; set; }
    public DbSet<OrderItems> OrderItems { get; set; }
    public DbSet<OrderCoupons> OrderCoupons { get; set; }
    public DbSet<Users> Users { get; set; }
    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);
        // Áp dụng tất cả IEntityTypeConfiguration trong assembly này
        // Không cần gọi từng file một — tự động scan
        mb.HasDefaultSchema("core"); 
        mb.ApplyConfigurationsFromAssembly(typeof(CoreDbContext).Assembly);
    }
}