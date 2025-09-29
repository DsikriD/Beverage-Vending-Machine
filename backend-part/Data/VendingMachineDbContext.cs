using BeverageVendingMachine.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BeverageVendingMachine.API.Data
{
    public class VendingMachineDbContext : DbContext
    {
        public VendingMachineDbContext(DbContextOptions<VendingMachineDbContext> options) : base(options)
        {
        }
        
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Coin> Coins { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Brand configuration
            modelBuilder.Entity<Brand>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(200);
                
                entity.HasIndex(e => e.Name).IsUnique();
            });
            
            // Product configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                
                entity.HasIndex(e => e.IsAvailable);
                
                entity.HasOne(e => e.Brand)
                    .WithMany(b => b.Products)
                    .HasForeignKey(e => e.BrandId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            
            // Order configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CustomerEmail).HasMaxLength(200);
                entity.Property(e => e.CustomerPhone).HasMaxLength(20);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(10,2)");
                
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.Status);
            });
            
            // OrderItem configuration
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.BrandName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(10,2)");
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(10,2)");
                
                entity.HasOne(e => e.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            // Coin configuration
            modelBuilder.Entity<Coin>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nominal).IsRequired();
                entity.Property(e => e.Count).IsRequired();
                
                entity.HasIndex(e => e.Nominal).IsUnique();
            });
            
            // Seed data
            SeedData(modelBuilder);
        }
        
        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Brands
            modelBuilder.Entity<Brand>().HasData(
                new Brand { Id = 1, Name = "Coca-Cola", Description = "The Coca-Cola Company", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Brand { Id = 2, Name = "Fanta", Description = "Fanta Orange", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Brand { Id = 3, Name = "Sprite", Description = "Sprite Lemon-Lime", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Brand { Id = 4, Name = "Dr. Pepper", Description = "Dr. Pepper", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );

            // Seed Products (не менее 8 товаров согласно ТЗ)
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Напиток газированный Coca-Cola",
                    BrandId = 1,
                    Price = 105.00m,
                    ImageUrl = "/images/coca-cola.jpg",
                    IsAvailable = true,
                    StockQuantity = 10,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = 2,
                    Name = "Напиток газированный Fanta",
                    BrandId = 2,
                    Price = 98.00m,
                    ImageUrl = "/images/fanta.jpg",
                    IsAvailable = true,
                    StockQuantity = 15,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = 3,
                    Name = "Напиток газированный Sprite",
                    BrandId = 3,
                    Price = 83.00m,
                    ImageUrl = "/images/sprite.jpg",
                    IsAvailable = true,
                    StockQuantity = 20,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = 4,
                    Name = "Напиток газированный Dr. Pepper Zero",
                    BrandId = 4,
                    Price = 110.00m,
                    ImageUrl = "/images/dr-pepper.jpg",
                    IsAvailable = false,
                    StockQuantity = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = 5,
                    Name = "Напиток газированный Coca-Cola",
                    BrandId = 1,
                    Price = 105.00m,
                    ImageUrl = "/images/coca-cola.jpg",
                    IsAvailable = true,
                    StockQuantity = 8,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = 6,
                    Name = "Напиток газированный Fanta",
                    BrandId = 2,
                    Price = 98.00m,
                    ImageUrl = "/images/fanta.jpg",
                    IsAvailable = true,
                    StockQuantity = 12,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = 7,
                    Name = "Напиток газированный Sprite",
                    BrandId = 3,
                    Price = 83.00m,
                    ImageUrl = "/images/sprite.jpg",
                    IsAvailable = true,
                    StockQuantity = 18,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = 8,
                    Name = "Напиток газированный Dr. Pepper Zero",
                    BrandId = 4,
                    Price = 110.00m,
                    ImageUrl = "/images/dr-pepper.jpg",
                    IsAvailable = true,
                    StockQuantity = 5,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );

            // Seed Coins (номиналы 1, 2, 5, 10 рублей согласно ТЗ)
            modelBuilder.Entity<Coin>().HasData(
                new Coin { Id = 1, Nominal = 1, Count = 100, IsAvailable = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Coin { Id = 2, Nominal = 2, Count = 50, IsAvailable = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Coin { Id = 3, Nominal = 5, Count = 30, IsAvailable = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Coin { Id = 4, Nominal = 10, Count = 20, IsAvailable = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );
        }
    }
}
