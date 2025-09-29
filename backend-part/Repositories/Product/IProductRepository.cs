using BeverageVendingMachine.API.Models;
using BeverageVendingMachine.API.Repositories;

namespace BeverageVendingMachine.API.Repositories.Product
{
    public interface IProductRepository : IBasicDatabaseRepository<Models.Product>
    {
        Task<IEnumerable<Models.Product>> GetByBrandAsync(int brandId);
        Task<IEnumerable<Models.Product>> GetAvailableAsync();
        Task<IEnumerable<Models.Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<IEnumerable<Models.Product>> GetFilteredAsync(int? brandId, decimal? maxPrice, bool? available);
        Task<Models.Product?> GetByNameAndBrandAsync(string name, int brandId);
    }
}