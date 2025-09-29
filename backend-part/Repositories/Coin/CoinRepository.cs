using BeverageVendingMachine.API.Data;
using BeverageVendingMachine.API.Models;
using BeverageVendingMachine.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BeverageVendingMachine.API.Repositories.Coin
{
    public class CoinRepository : BasicDatabaseRepository<Models.Coin>, ICoinRepository
    {
        public CoinRepository(VendingMachineDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Models.Coin>> GetAllAsync()
        {
            return await _dbSet
                .Where(c => c.IsAvailable)
                .OrderByDescending(c => c.Nominal)
                .ToListAsync();
        }

        public async Task<Models.Coin?> GetByNominalAsync(int nominal)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Nominal == nominal);
        }

        public async Task<Dictionary<int, int>> GetChangeAsync(decimal amount)
        {
            var change = new Dictionary<int, int>();
            var coins = await GetAllAsync();
            var remainingAmount = (int)(amount * 100); // Convert to kopecks

            foreach (var coin in coins)
            {
                var coinValue = coin.Nominal;
                var coinCount = Math.Min(coin.Count, remainingAmount / coinValue);
                
                if (coinCount > 0)
                {
                    change[coinValue] = coinCount;
                    remainingAmount -= coinCount * coinValue;
                }
            }

            return remainingAmount == 0 ? change : new Dictionary<int, int>();
        }

        public async Task<bool> CanGiveChangeAsync(decimal amount)
        {
            var change = await GetChangeAsync(amount);
            return change.Count > 0;
        }
    }
}