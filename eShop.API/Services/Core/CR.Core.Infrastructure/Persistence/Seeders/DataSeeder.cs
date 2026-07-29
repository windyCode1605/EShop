using CR.Core.Domain.Catalog;
using CR.Core.Domain.User;
using CR.Core.Domain.SysVar;
using CR.Core.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.Infrastructure.Persistence.Seeders
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(CoreDbContext db)
        {
            // 1. Roles
            if (!await db.Roles.AnyAsync())
            {
                var roles = new List<Role>
                {
                    new Role { Name = "SuperAdmin", Description = "Quản trị viên cấp cao", Status = 1 },
                    new Role { Name = "Admin", Description = "Quản trị viên", Status = 1 },
                    new Role { Name = "Moderator", Description = "Người kiểm duyệt", Status = 1 }
                };
                db.Roles.AddRange(roles);
                await db.SaveChangesAsync();
            }

            // 2. Users
            if (!await db.Users.AnyAsync(u => u.Email == "maiquangnguyenkt2004@gmail.com"))
            {
                var superAdminRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "SuperAdmin");
                if (superAdminRole != null)
                {
                    var superAdminUser = new Users
                    {
                        Username = "maiquangnguyenkt2004",
                        Email = "maiquangnguyenkt2004@gmail.com",
                        Phone = "0900000000",
                        PasswordHash = "AQAAAAIAAYagAAAAEM0A/pTJksE81875vdd2CmmJojUYgmLlBZgrW1n5w+eHzajj8ZSU1x8+0qSmHC0tsw==",
                        UserType = (CR.Constants.Core.Users.UserTypeEnum)1,
                        Status = 1,
                        IsTempPassword = false,
                        Profile = new UserProfile { FullName = "Mai Quang Nguyen", PhoneNumber = "0900000000", Gender = (CR.Constants.Core.Users.GenderTypes)1 },
                        UserRoles = new List<UserRole> { new UserRole { Role = superAdminRole } } // SuperAdmin
                    };
                    db.Users.Add(superAdminUser);
                    await db.SaveChangesAsync();
                }
            }

            if (await db.Products.AnyAsync()) return;

            var users = new List<Users>
            {
                new Users {
                    Username = "admin01", Email = "admin01@eshop.com", Phone = "0900000001", PasswordHash = "hashed_pwd_1", UserType = (CR.Constants.Core.Users.UserTypeEnum)1, Status = 1, IsTempPassword = false,
                    Profile = new UserProfile { FullName = "Admin 1", PhoneNumber = "0900000001", Gender = (CR.Constants.Core.Users.GenderTypes)1 }
                },
                new Users {
                    Username = "user02", Email = "user02@eshop.com", Phone = "0900000002", PasswordHash = "hashed_pwd_2", UserType = (CR.Constants.Core.Users.UserTypeEnum)2, Status = 1, IsTempPassword = false,
                    Profile = new UserProfile { FullName = "User 2", PhoneNumber = "0900000002", Gender = (CR.Constants.Core.Users.GenderTypes)0 }
                }
            };
            if (!await db.Users.AnyAsync()) db.Users.AddRange(users);

            // 3. SysVars
            var sysVars = new List<SysVar>
            {
                new SysVar { GrName = "Config", VarName = "SiteName", VarValue = "EShop", VarDesc = "Tên website" },
                new SysVar { GrName = "Payment", VarName = "VAT", VarValue = "10", VarDesc = "Thuế GTGT" },
                new SysVar { GrName = "Shipping", VarName = "BaseFee", VarValue = "30000", VarDesc = "Phí ship cơ bản" },
                new SysVar { GrName = "AUTHMAXTURN", VarName = "LOGINMAXTURN", VarValue = "5", VarDesc = "Số lần đăng nhập sai tối đa" }
            };
            if (!await db.SysVars.AnyAsync()) db.SysVars.AddRange(sysVars);

            // 4. Categories
            var categories = new List<Category>
            {
                new Category { Name = "Điện tử", Slug = "dien-tu" },
                new Category { Name = "Thời trang", Slug = "thoi-trang" },
                new Category { Name = "Sách", Slug = "sach" }
            };
            if (!await db.Categories.AnyAsync())
            {
                db.Categories.AddRange(categories);
                await db.SaveChangesAsync();
            }

            var catDienTuId = categories[0].Id;
            var subCategories = new List<Category>
            {
                new Category { ParentId = catDienTuId, Name = "Điện thoại", Slug = "dien-thoai" },
                new Category { ParentId = catDienTuId, Name = "Laptop", Slug = "laptop" }
            };
            db.Categories.AddRange(subCategories);
            await db.SaveChangesAsync();

            var catDienThoaiId = subCategories[0].Id;
            var catLaptopId = subCategories[1].Id;

            // 5. Products
            var products = new List<Product>
            {
                new Product { CategoryId = catDienThoaiId, Name = "Apple iPhone 15 Pro Max 256GB", Slug = "iphone-15-pro-max-256gb", BasePrice = 29990000, Description = "Điện thoại flagship mới nhất từ Apple." },
                new Product { CategoryId = catDienThoaiId, Name = "Samsung Galaxy S24 Ultra 5G", Slug = "samsung-galaxy-s24-ultra", BasePrice = 28500000, Description = "Tích hợp AI thông minh, camera 200MP zoom quang 10x." },
                new Product { CategoryId = catLaptopId, Name = "Apple MacBook Air M3 2024", Slug = "macbook-air-m3", BasePrice = 27990000, Description = "Laptop mỏng nhẹ, chip M3 hiệu năng cao." },
                new Product { CategoryId = catLaptopId, Name = "Laptop Dell XPS 15 9530", Slug = "dell-xps-15-9530", BasePrice = 45000000, Description = "Màn hình OLED, thiết kế viền siêu mỏng dành cho doanh nhân." }
            };
            db.Products.AddRange(products);
            await db.SaveChangesAsync();

            // 6. Product Variants & Images
            db.ProductVariants.AddRange(new List<ProductVariant>
            {
                new ProductVariant { ProductId = products[0].Id, SKU = "IP15PM-256-TI", StockQuantity = 50 },
                new ProductVariant { ProductId = products[1].Id, SKU = "S24U-256-BLK", StockQuantity = 60 },
                new ProductVariant { ProductId = products[2].Id, SKU = "MBA-M3-14-SLV", StockQuantity = 45 },
                new ProductVariant { ProductId = products[3].Id, SKU = "XPS-15-OLED", StockQuantity = 30 }
            });

            db.ProductImages.AddRange(new List<ProductImage>
            {
                new ProductImage { ProductId = products[0].Id, Url = "img11.jpg", IsPrimary = true },
                new ProductImage { ProductId = products[1].Id, Url = "img12.jpg", IsPrimary = true },
                new ProductImage { ProductId = products[2].Id, Url = "img13.jpg", IsPrimary = true },
                new ProductImage { ProductId = products[3].Id, Url = "img14.jpg", IsPrimary = true }
            });

            await db.SaveChangesAsync();
        }
    }
}
