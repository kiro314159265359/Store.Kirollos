using Domain.Contracts;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Persistence
{
    public class DbInitializer(StoreDbContext _context) : IDbInitializer
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
            }
            catch (Exception ex) 
            {
                throw;
            }
        }
    }
}
//   \Data\Seeding\types.json
// ..\Infrastructure\Persistence\Data\Seeding\types.json