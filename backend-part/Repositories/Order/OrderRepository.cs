using BeverageVendingMachine.API.Data;
using BeverageVendingMachine.API.Models;
using BeverageVendingMachine.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BeverageVendingMachine.API.Repositories.Order
{
    public class OrderRepository : BasicDatabaseRepository<Models.Order>, IOrderRepository
    {
        public OrderRepository(VendingMachineDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Models.Order>> GetAllAsync()
        {
            return await _dbSet
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public override async Task<Models.Order?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Models.Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(o => o.OrderItems)
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.Order>> GetByStatusAsync(OrderStatus status)
        {
            return await _dbSet
                .Include(o => o.OrderItems)
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
    }
}
