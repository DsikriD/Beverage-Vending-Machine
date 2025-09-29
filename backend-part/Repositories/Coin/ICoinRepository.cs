using BeverageVendingMachine.API.Models;
using BeverageVendingMachine.API.Repositories;

namespace BeverageVendingMachine.API.Repositories.Coin
{
    public interface ICoinRepository : IBasicDatabaseRepository<Models.Coin>
    {
        Task<Models.Coin?> GetByNominalAsync(int nominal);
        Task<Dictionary<int, int>> GetChangeAsync(decimal amount);
        Task<bool> CanGiveChangeAsync(decimal amount);
    }
}