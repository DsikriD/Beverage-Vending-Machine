using BeverageVendingMachine.API.Models;
using BeverageVendingMachine.API.Models.DTOs;
using BeverageVendingMachine.API.Repositories;
using BeverageVendingMachine.API.Repositories.Product;
using BeverageVendingMachine.API.Repositories.Brand;

namespace BeverageVendingMachine.API.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IBrandRepository _brandRepository;

        public ProductService(IProductRepository productRepository, IBrandRepository brandRepository)
        {
            _productRepository = productRepository;
            _brandRepository = brandRepository;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync(int? brandId, decimal? maxPrice, bool? available)
        {
            var products = await _productRepository.GetFilteredAsync(brandId, maxPrice, available);
            
            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Brand = p.Brand.Name,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                IsAvailable = p.IsAvailable,
                StockQuantity = p.StockQuantity
            });
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return null;

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Brand = product.Brand.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                IsAvailable = product.IsAvailable,
                StockQuantity = product.StockQuantity
            };
        }

        public async Task<IEnumerable<string>> GetBrandsAsync()
        {
            var brands = await _brandRepository.GetAllAsync();
            return brands.Select(b => b.Name);
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            var brand = await _brandRepository.GetByNameAsync(createProductDto.Brand);
            if (brand == null)
            {
                throw new ArgumentException($"Brand '{createProductDto.Brand}' not found");
            }

            var product = new Product
            {
                Name = createProductDto.Name,
                BrandId = brand.Id,
                Price = createProductDto.Price,
                ImageUrl = createProductDto.ImageUrl,
                StockQuantity = createProductDto.StockQuantity,
                IsAvailable = createProductDto.StockQuantity > 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdProduct = await _productRepository.CreateAsync(product);
            
            return new ProductDto
            {
                Id = createdProduct.Id,
                Name = createdProduct.Name,
                Brand = brand.Name,
                Price = createdProduct.Price,
                ImageUrl = createdProduct.ImageUrl,
                IsAvailable = createdProduct.IsAvailable,
                StockQuantity = createdProduct.StockQuantity
            };
        }

        public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new ArgumentException($"Product with ID {id} not found");
            }

            if (updateProductDto.Name != null)
                product.Name = updateProductDto.Name;
            if (updateProductDto.Brand != null)
            {
                var brand = await _brandRepository.GetByNameAsync(updateProductDto.Brand);
                if (brand == null)
                {
                    throw new ArgumentException($"Brand '{updateProductDto.Brand}' not found");
                }
                product.BrandId = brand.Id;
            }
            if (updateProductDto.Price.HasValue)
                product.Price = updateProductDto.Price.Value;
            if (updateProductDto.ImageUrl != null)
                product.ImageUrl = updateProductDto.ImageUrl;
            if (updateProductDto.IsAvailable.HasValue)
                product.IsAvailable = updateProductDto.IsAvailable.Value;
            if (updateProductDto.StockQuantity.HasValue)
                product.StockQuantity = updateProductDto.StockQuantity.Value;

            var updatedProduct = await _productRepository.UpdateAsync(product);
            
            return new ProductDto
            {
                Id = updatedProduct.Id,
                Name = updatedProduct.Name,
                Brand = updatedProduct.Brand.Name,
                Price = updatedProduct.Price,
                ImageUrl = updatedProduct.ImageUrl,
                IsAvailable = updatedProduct.IsAvailable,
                StockQuantity = updatedProduct.StockQuantity
            };
        }

        public async Task DeleteProductAsync(int id)
        {
            await _productRepository.DeleteAsync(id);
        }

        public async Task<bool> ValidateProductAvailabilityAsync(int productId, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            return product != null && product.IsAvailable && product.StockQuantity >= quantity;
        }

        public async Task UpdateProductStockAsync(int productId, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product != null)
            {
                product.StockQuantity -= quantity;
                product.IsAvailable = product.StockQuantity > 0;
                await _productRepository.UpdateAsync(product);
            }
        }

        public async Task<List<ProductDto>> ImportProductsAsync(List<ImportProductDto> importProducts)
        {
            var createdProducts = new List<ProductDto>();
            
            foreach (var importProduct in importProducts)
            {
                // Найти или создать бренд
                var brand = await _brandRepository.GetByNameAsync(importProduct.BrandName);
                if (brand == null)
                {
                    brand = new Models.Brand
                    {
                        Name = importProduct.BrandName,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _brandRepository.CreateAsync(brand);
                }

                // Проверить, существует ли уже продукт с таким именем и брендом
                var existingProduct = await _productRepository.GetByNameAndBrandAsync(importProduct.Name, brand.Id);
                if (existingProduct != null)
                {
                    // Обновить существующий продукт
                    existingProduct.Price = importProduct.Price;
                    existingProduct.ImageUrl = importProduct.ImageUrl ?? existingProduct.ImageUrl;
                    existingProduct.StockQuantity = importProduct.StockQuantity;
                    existingProduct.IsAvailable = importProduct.StockQuantity > 0;
                    existingProduct.UpdatedAt = DateTime.UtcNow;
                    
                    var updatedProduct = await _productRepository.UpdateAsync(existingProduct);
                    
                    createdProducts.Add(new ProductDto
                    {
                        Id = updatedProduct.Id,
                        Name = updatedProduct.Name,
                        Brand = brand.Name,
                        Price = updatedProduct.Price,
                        ImageUrl = updatedProduct.ImageUrl,
                        IsAvailable = updatedProduct.IsAvailable,
                        StockQuantity = updatedProduct.StockQuantity
                    });
                }
                else
                {
                    // Создать новый продукт
                    var product = new Models.Product
                    {
                        Name = importProduct.Name,
                        BrandId = brand.Id,
                        Price = importProduct.Price,
                        ImageUrl = importProduct.ImageUrl,
                        StockQuantity = importProduct.StockQuantity,
                        IsAvailable = importProduct.StockQuantity > 0,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    var createdProduct = await _productRepository.CreateAsync(product);
                    
                    createdProducts.Add(new ProductDto
                    {
                        Id = createdProduct.Id,
                        Name = createdProduct.Name,
                        Brand = brand.Name,
                        Price = createdProduct.Price,
                        ImageUrl = createdProduct.ImageUrl,
                        IsAvailable = createdProduct.IsAvailable,
                        StockQuantity = createdProduct.StockQuantity
                    });
                }
            }
            
            return createdProducts;
        }
    }
}
