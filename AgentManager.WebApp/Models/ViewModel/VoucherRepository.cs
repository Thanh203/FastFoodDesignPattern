using FastFoodSystem.WebApp.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace FastFoodSystem.WebApp.Models.ViewModel
{
    public class VoucherRepository : GenericRepository<FFSVoucher>
    {
        public VoucherRepository(FastFoodSystemDbContext context): base(context) 
        {

        }


        public override async Task<FFSVoucher> GetByIdAsync(string id)
        {
            return await _context.FFSVouchers.FindAsync(id);
        }

        public override async Task<List<FFSVoucher>> GetAllAsync()
        {
            return await _context.FFSVouchers.ToListAsync();
        }

        public override async Task AddAsync(FFSVoucher voucher)
        {
            _context.FFSVouchers.Add(voucher);
            await _context.SaveChangesAsync();
        }

        public override async Task UpdateAsync(FFSVoucher voucher, string id)
        {
            var crvoucher = await _context.FFSVouchers.FindAsync(id);
            _context.Entry(crvoucher).CurrentValues.SetValues(voucher);
            await _context.SaveChangesAsync();
        }

        public override async Task DeleteAsync(string id)
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
