using BeverageVendingMachine.API.Data;
using BeverageVendingMachine.API.Models;
using BeverageVendingMachine.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BeverageVendingMachine.API.Repositories.Product
{
    public class ProductRepository : BasicDatabaseRepository<Models.Product>, IProductRepository
    {
        public ProductRepository(VendingMachineDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Models.Product>> GetAllAsync()
        {
            return await _dbSet
                .Include(p => p.Brand)
                .ToListAsync();
        }

        public override async Task<Models.Product?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Models.Product>> GetByBrandAsync(int brandId)
        {
            return await _dbSet
                .Include(p => p.Brand)
                .Where(p => p.BrandId == brandId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.Product>> GetAvailableAsync()
        {
            return await _dbSet
                .Include(p => p.Brand)
                .Where(p => p.IsAvailable && p.StockQuantity > 0)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _dbSet
                .Include(p => p.Brand)
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.Product>> GetFilteredAsync(int? brandId, decimal? maxPrice, bool? available)
        {
            var query = _dbSet.Include(p => p.Brand).AsQueryable();

            if (brandId.HasValue)
                query = query.Where(p => p.BrandId == brandId.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            if (available.HasValue)
                query = query.Where(p => p.IsAvailable == available.Value);

            return await query.ToListAsync();
        }

        public async Task<Models.Product?> GetByNameAndBrandAsync(string name, int brandId)
        {
            return await _dbSet
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.Name == name && p.BrandId == brandId);
        }
    }
}
