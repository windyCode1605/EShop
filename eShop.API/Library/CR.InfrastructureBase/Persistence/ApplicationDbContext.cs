// Library/CR.InfrastructureBase/Persistence/ApplicationDbContext.cs
// Base DbContext — mỗi service tạo DbContext riêng kế thừa từ đây
// TUser là entity User của service đó (không chia sẻ User table giữa services)
using CR.EntitiesBase;
using Microsoft.EntityFrameworkCore;

public abstract class ApplicationDbContext<TUser> : DbContext
    where TUser : class
{
    protected ApplicationDbContext(DbContextOptions options) : base(options) { }

    // Hook: tự động set CreatedAt/UpdatedAt trước khi SaveChanges
    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = DateTime.UtcNow;

            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
        return base.SaveChangesAsync(ct);
    }
}