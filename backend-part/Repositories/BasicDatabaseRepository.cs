using BeverageVendingMachine.API.Data;
using BeverageVendingMachine.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BeverageVendingMachine.API.Repositories
{
    public class BasicDatabaseRepository<T> : IBasicDatabaseRepository<T> where T : class
    {
        protected readonly VendingMachineDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BasicDatabaseRepository(VendingMachineDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            // Обновляем UpdatedAt для сущностей, которые имеют это поле
            if (entity is IHasUpdatedAt hasUpdatedAt)
            {
                hasUpdatedAt.UpdatedAt = DateTime.UtcNow;
            }

            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public virtual async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet.FindAsync(id) != null;
        }
    }

    // Интерфейс для сущностей с полем UpdatedAt
    public interface IHasUpdatedAt
    {
        DateTime UpdatedAt { get; set; }
    }
}
