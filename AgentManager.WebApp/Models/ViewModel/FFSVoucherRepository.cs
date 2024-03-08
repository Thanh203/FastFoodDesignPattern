using FastFoodSystem.WebApp.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace FastFoodSystem.WebApp.Models.ViewModel
{
    public interface IVoucherRepository
    {
        Task<FFSVoucher> GetByIdAsync(string id);
        Task<List<FFSVoucher>> GetAllAsync();
        Task AddAsync(FFSVoucher voucher);
        Task UpdateAsync(FFSVoucher voucher, string id);
        Task DeleteAsync(string id);
    }

    public class FFSVoucherRepository: IVoucherRepository
    {
        private readonly FastFoodSystemDbContext _context;

        public FFSVoucherRepository(FastFoodSystemDbContext context)
        {
            _context = context;
        }

        public async Task<FFSVoucher> GetByIdAsync(string id)
        {
            return await _context.FFSVouchers.FindAsync(id);
        }

        public async Task<List<FFSVoucher>> GetAllAsync()
        {
            return await _context.FFSVouchers.ToListAsync();
        }

        public async Task AddAsync(FFSVoucher voucher)
        {
            _context.FFSVouchers.Add(voucher);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FFSVoucher voucher, string id)
        {
            var crvoucher = await _context.FFSVouchers.FindAsync(id);
            _context.Entry(crvoucher).CurrentValues.SetValues(voucher);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var voucher = await _context.FFSVouchers.FindAsync(id);
            if (voucher != null)
            {
                _context.FFSVouchers.Remove(voucher);
                await _context.SaveChangesAsync();
            }
        }
    }
}
