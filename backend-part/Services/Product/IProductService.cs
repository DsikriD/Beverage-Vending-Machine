using BeverageVendingMachine.API.Models;
using BeverageVendingMachine.API.Models.DTOs;

namespace BeverageVendingMachine.API.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync(int? brandId, decimal? maxPrice, bool? available);
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<IEnumerable<string>> GetBrandsAsync();
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
        Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto updateProductDto);
        Task DeleteProductAsync(int id);
        Task<bool> ValidateProductAvailabilityAsync(int productId, int quantity);
        Task UpdateProductStockAsync(int productId, int quantity);
        Task<List<ProductDto>> ImportProductsAsync(List<ImportProductDto> importProducts);
    }
}
