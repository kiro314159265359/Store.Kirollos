using Domain.Contracts;
using Domain.Models;
using Domain.Models.Identity;
using Domain.Models.OrderModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Persistence.Data;
using Persistence.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Persistence
{
    public class DbInitializer(
        StoreDbContext _context,
        StoreIdentityDbContext _identityDbContext,
        UserManager<AppUser> _userManager,
        RoleManager<IdentityRole> _roleManager) : IDbInitializer
    {
        public async Task InitializeAsync()
        {
            try
            {
                // Create Database if not found
                // Apply any pending migrations
                if (_context.Database.GetPendingMigrations().Any()) // true if there is migrations that isn't applied
                {
                    await _context.Database.MigrateAsync();
                }
                // Data Seeding
                // Seeding ProductTypes
                if (!_context.ProductTypes.Any())
                {
                    // 1. Read all data from json file as string
                    var typesData = await File.ReadAllTextAsync(@"..\Infrastructure\Persistence\Data\Seeding\types.json");

                    // 2. Convert type from string to object[List<ProductTypes>]
                    // Serializing json to string
                    var types = JsonSerializer.Deserialize<List<ProductType>>(typesData);

                    // 3. Add List<ProductTypes> to database
                    if (types is not null && types.Any())
                    {
                        await _context.ProductTypes.AddRangeAsync(types);
                        await _context.SaveChangesAsync();
                    }
                }
                // Seeding ProductBrand
                if (!_context.ProductBrands.Any())
                {
                    // 1. Read all data from json file as string
                    var brandsData = await File.ReadAllTextAsync(@"..\Infrastructure\Persistence\Data\Seeding\brands.json");

                    // 2. Convert type from string to object[List<ProductBrand>]
                    // Serializing json to string
                    var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);

                    // 3. Add List<ProductTypes> to database
                    if (brands is not null && brands.Any())
                    {
                        await _context.ProductBrands.AddRangeAsync(brands);
                        await _context.SaveChangesAsync();
                    }
                }
                // Seeding Product
                if (!_context.Products.Any())
                {
                    // 1. Read all data from json file as string
                    var productsData = await File.ReadAllTextAsync(@"..\Infrastructure\Persistence\Data\Seeding\products.json");

                    // 2. Convert type from string to object[List<ProductBrand>]
                    // Serializing json to string
                    var products = JsonSerializer.Deserialize<List<Product>>(productsData);

                    // 3. Add List<ProductTypes> to database
                    if (products is not null && products.Any())
                    {
                        await _context.Products.AddRangeAsync(products);
                        await _context.SaveChangesAsync();
                    }
                }

                if (!_context.DeliveryMethods.Any())
                {
                    // 1. Read all data from json file as string
                    var deliveryData = await File.ReadAllTextAsync(@"..\Infrastructure\Persistence\Data\Seeding\delivery.json");

                    // 2. Convert type from string to object[List<DeliveryMethods>]
                    // Serializing json to string
                    var deliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryData);

                    // 3. Add List<ProductTypes> to database
                    if (deliveryMethods is not null && deliveryMethods.Any())
                    {
                        await _context.DeliveryMethods.AddRangeAsync(deliveryMethods);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex) 
            {
                throw;
            }
        }

        public async Task InitializeIdentityAsync()
        {
            if (_identityDbContext.Database.GetPendingMigrations().Any())
            {
                await _identityDbContext.Database.MigrateAsync();
            }

            if (!_roleManager.Roles.Any()) 
            {
                await _roleManager.CreateAsync(new IdentityRole()
                {
                    Name = "Admin"
                });
                await _roleManager.CreateAsync(new IdentityRole()
                {
                    Name = "SuperAdmin"
                });
            }
            // Seeding
            if (!_userManager.Users.Any())
            {
                var superAdminUser = new AppUser()
                {
                    DisplayName = "Super Admin",
                    Email = "SuperAdmin@gmail.com",
                    UserName = "SuperAdmin",
                    PhoneNumber = "012345667"
                };

                var AdminUser = new AppUser()
                {
                    DisplayName = "Admin",
                    Email = "Admin@gmail.com",
                    UserName = "Admin",
                    PhoneNumber = "012345667"
                };
                await _userManager.CreateAsync(superAdminUser, "P@ssw0rd");
                await _userManager.CreateAsync(AdminUser, "P@ssw0rd");

                await _userManager.AddToRoleAsync(superAdminUser, "SuperAdmin");
                await _userManager.AddToRoleAsync(AdminUser, "Admin");
            }
        }
    }
}
//   \Data\Seeding\types.json
// ..\Infrastructure\Persistence\Data\Seeding\types.json