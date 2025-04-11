using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Data.Configurations
{
    internal class ProductConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasOne(b => b.ProductBrand)
                   .WithMany()
                   .HasForeignKey(b => b.BrandId);

            builder.HasOne(b => b.ProductType)
                   .WithMany()
                   .HasForeignKey(b => b.TypeId);

            builder.Property(P => P.Price)
                   .HasColumnType("decimal(18,2)");
        }
    }
}
