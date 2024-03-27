using FastFoodSystem.WebApp.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace FastFoodSystem.WebApp.Models.ViewModel
{
    public abstract class GenericRepository<T> : IRepository<T> where T : class
    {
        protected readonly FastFoodSystemDbContext _context;

        public GenericRepository(FastFoodSystemDbContext context)
        {
            _context = context;
        }

        public virtual async Task<T> GetByIdAsync(string id)
        {
            return await _context.FindAsync<T>(id);
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public virtual async Task AddAsync(T voucher)
        {
            _context.Add(voucher);
            await _context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(T voucher, string id)
        {
            var crvoucher = await _context.FindAsync<T>(id);
            _context.Entry(crvoucher).CurrentValues.SetValues(voucher);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(string id)
        {
            var voucher = await _context.FindAsync<T>(id);
            if (voucher != null)
            {
                _context.Set<T>().Remove(voucher);
                await _context.SaveChangesAsync();
            }
        }
    }
}
