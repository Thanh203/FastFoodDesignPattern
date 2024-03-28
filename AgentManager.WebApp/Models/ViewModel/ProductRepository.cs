using FastFoodSystem.WebApp.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace FastFoodSystem.WebApp.Models.ViewModel
{
    public class ProductRepository : GenericRepository<FFSProduct>
    {
        public ProductRepository(FastFoodSystemDbContext context): base(context) 
        {

        }

        public override async Task<FFSProduct> GetByIdAsync(string id)
        {
            return await _context.FFSProducts.FindAsync(id);
        }

        public override async Task<List<FFSProduct>> GetAllAsync()
        {
            return await _context.FFSProducts.ToListAsync();
        }

        public override async Task AddAsync(FFSProduct product)
        {
            _context.FFSProducts.Add(product);
            await _context.SaveChangesAsync();
        }

        public override async Task UpdateAsync(FFSProduct product, string id)
        {
            var crproduct = await _context.FFSProducts.FindAsync(id);
            _context.Entry(crproduct).CurrentValues.SetValues(product);
            await _context.SaveChangesAsync();
        }

        public override async Task DeleteAsync(string id)
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

