using BeverageVendingMachine.API.Models;
using BeverageVendingMachine.API.Repositories;

namespace BeverageVendingMachine.API.Repositories.Order
{
    public interface IOrderRepository : IBasicDatabaseRepository<Models.Order>
    {
        Task<IEnumerable<Models.Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Models.Order>> GetByStatusAsync(OrderStatus status);
    }
}