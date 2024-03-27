using FastFoodSystem.WebApp.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace FastFoodSystem.WebApp.Models.ViewModel
{
    public class CategoryRepository : GenericRepository<FFSProductCategory>
    {
        public CategoryRepository(FastFoodSystemDbContext context) : base(context) 
        {
           
        }

        public override async Task<FFSProductCategory> GetByIdAsync(string id)
        {
            return await _context.FFSProductCategories.FindAsync(id);
        }

        public override async Task<List<FFSProductCategory>> GetAllAsync()
        {
            return await _context.FFSProductCategories.ToListAsync();
        }

        public override async Task AddAsync(FFSProductCategory category)
        {
            _context.FFSProductCategories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async override Task UpdateAsync(FFSProductCategory category, string id)
        {
            var crcategory = await _context.FFSProductCategories.FindAsync(id);
            _context.Entry(crcategory).CurrentValues.SetValues(category);
            await _context.SaveChangesAsync();
        }

        public override async Task DeleteAsync(string id)
        {
            var category = await _context.FFSProductCategories.FindAsync(id);
            if (category != null)
            {
                _context.FFSProductCategories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}
