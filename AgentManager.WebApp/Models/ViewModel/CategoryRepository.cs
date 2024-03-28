using FastFoodSystem.WebApp.Models.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace FastFoodSystem.WebApp.Models.ViewModel
{
    public interface ICategoryRepository
    {
        Task<FFSProductCategory> GetByIdAsync(string id);
        Task<List<FFSProductCategory>> GetAllAsync();
        Task AddAsync(FFSProductCategory category);
        Task UpdateAsync(FFSProductCategory category, string id);
        Task DeleteAsync(string id);
    }
    public class CategoryRepository : ICategoryRepository
    {
        private readonly FastFoodSystemDbContext _context;

        public CategoryRepository(FastFoodSystemDbContext context)
        {
            _context = context;
        }

        public async Task<FFSProductCategory> GetByIdAsync(string id)
        {
            return await _context.FFSProductCategories.FindAsync(id);
        }

        public async Task<List<FFSProductCategory>> GetAllAsync()
        {
            return await _context.FFSProductCategories.ToListAsync();
        }

        public async Task AddAsync(FFSProductCategory category)
        {
            _context.FFSProductCategories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FFSProductCategory category, string id)
        {
            var crcategory = await _context.FFSProductCategories.FindAsync(id);
            _context.Entry(crcategory).CurrentValues.SetValues(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
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
