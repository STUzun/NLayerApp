using Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Seeds
{
    internal class ProductSeed : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasData(
                new Product
                {
                    Id = 1,
                    Name = "Dolma Kalemler",
                    CategoryId = 1,
                    Price = 10,
                    Stock = 5,
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true,
                    IsDeleted = false
                },
                new Product
                {
                    Id = 2,
                    Name = "Kurşun Kalemler",
                    CategoryId = 1,
                    Price = 50,
                    Stock = 10,
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true,
                    IsDeleted = false
                },
                new Product
                {
                    Id = 3,
                    Name = "Dolma Kalemler",
                    CategoryId = 1,
                    Price = 100,
                    Stock = 20,
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true,
                    IsDeleted = false
                },
                new Product
                {
                    Id = 4,
                    Name = "80 Yaprak Çizgili Defter",
                    CategoryId = 2,
                    Price = 50,
                    Stock = 10,
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true,
                    IsDeleted = false
                });
                
        }

    }
}
