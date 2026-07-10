# Implement Permission Domain and Database Schema Configuration

Thêm Domain Model, Entity Framework Configuration và tích hợp schema cơ sở dữ liệu cho tính năng phân quyền (Permissions & RolePermissions) dựa trên tài liệu SQL được cung cấp.

## User Review Required

> [!IMPORTANT]
> The EF Core Migration `AddRolePermissions` will be created. I will embed your custom T-SQL script directly into the `Up()` method of the migration so that it runs exactly as you specified (creating tables with `IF NOT EXISTS` and inserting seed data). Please confirm if this is the desired approach, hoặc bạn muốn tự chạy script trên SSMS rồi mình chỉ cần update EF Core Models?

## Proposed Changes

---

### Domain Layer (eShop.API\Services\Core\CR.Core.Domain\User)

#### [NEW] [Permission.cs](file:///d:/PROJECT/EShop/eShop.API/Services/Core/CR.Core.Domain/User/Permission.cs)
- Create new `Permission` class mapped to `[dbo].[Permission]`.
- Properties: `PermissionKey` (PK), `DisplayName`, `PermissionGroup`, `Description`, `CreatedDate`, `CreatedBy`.
- Collection navigation to `RolePermission`.

#### [MODIFY] [RolePermission.cs](file:///d:/PROJECT/EShop/eShop.API/Services/Core/CR.Core.Domain/User/RolePermission.cs)
- Update to include foreign key configuration to `Permission`.
- Add `public Permission Permission { get; set; }` navigation property.

---

### Infrastructure Layer (eShop.API\Services\Core\CR.Core.Infrastructure\Persistence)

#### [NEW] [PermissionConfiguration.cs](file:///d:/PROJECT/EShop/eShop.API/Services/Core/CR.Core.Infrastructure/Persistence/Configurations/PermissionConfiguration.cs)
- Implement `IEntityTypeConfiguration<Permission>`.
- Configure primary key (`PermissionKey`).

#### [NEW] [RolePermissionConfiguration.cs](file:///d:/PROJECT/EShop/eShop.API/Services/Core/CR.Core.Infrastructure/Persistence/Configurations/RolePermissionConfiguration.cs)
- Configure the relationship `HasOne(x => x.Permission).WithMany().HasForeignKey(x => x.PermissionKey).OnDelete(DeleteBehavior.Cascade)`.

#### [MODIFY] [CoreDbContext.cs](file:///d:/PROJECT/EShop/eShop.API/Services/Core/CR.Core.Infrastructure/Persistence/CoreDbContext.cs)
- Add `public DbSet<Permission> Permissions { get; set; }`.

---

## Verification Plan

### Automated Tests
- Run `dotnet build` to ensure no compile errors in Domain and Infrastructure layers.
- Run `dotnet ef migrations add AddPermissionSchema` to generate the migration.

### Manual Verification
- Verify the generated migration file contains the correct custom SQL script in the `Up()` method.
- Run `dotnet ef database update` and check the SQL Server database to ensure the `Permission` table is created and data is seeded.
