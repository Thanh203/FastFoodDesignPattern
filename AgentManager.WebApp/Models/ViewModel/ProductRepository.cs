using FastFoodSystem.WebApp.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace FastFoodSystem.WebApp.Models.ViewModel
{
    public interface IProductRepository
    {
        Task<FFSProduct> GetByIdAsync(string id);
        Task<List<FFSProduct>> GetAllAsync();
        Task AddAsync(FFSProduct product);
        Task UpdateAsync(FFSProduct product, string id);
        Task DeleteAsync(string id);
    }
    public class ProductRepository : IProductRepository
    {
        private readonly FastFoodSystemDbContext _context;

        public ProductRepository(FastFoodSystemDbContext context)
        {
            _context = context;
        }

        public async Task<FFSProduct> GetByIdAsync(string id)
        {
            return await _context.FFSProducts.FindAsync(id);
        }

        public async Task<List<FFSProduct>> GetAllAsync()
        {
            return await _context.FFSProducts.ToListAsync();
        }

        public async Task AddAsync(FFSProduct product)
        {
            _context.FFSProducts.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FFSProduct product, string id)
        {
            var crproduct = await _context.FFSProducts.FindAsync(id);
            _context.Entry(crproduct).CurrentValues.SetValues(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var product = await _context.FFSProducts.FindAsync(id);
            if (product != null)
            {
                _context.FFSProducts.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}

