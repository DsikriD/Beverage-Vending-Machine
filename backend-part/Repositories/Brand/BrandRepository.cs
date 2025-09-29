using BeverageVendingMachine.API.Data;
using BeverageVendingMachine.API.Models;
using BeverageVendingMachine.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BeverageVendingMachine.API.Repositories.Brand
{
    public class BrandRepository : BasicDatabaseRepository<Models.Brand>, IBrandRepository
    {
        public BrandRepository(VendingMachineDbContext context) : base(context)
        {
        }

        public async Task<Models.Brand?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(b => b.Name == name);
        }
    }
}
