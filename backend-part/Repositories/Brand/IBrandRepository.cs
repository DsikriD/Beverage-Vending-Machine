using BeverageVendingMachine.API.Models;
using BeverageVendingMachine.API.Repositories;

namespace BeverageVendingMachine.API.Repositories.Brand
{
    public interface IBrandRepository : IBasicDatabaseRepository<Models.Brand>
    {
        Task<Models.Brand?> GetByNameAsync(string name);
    }
}