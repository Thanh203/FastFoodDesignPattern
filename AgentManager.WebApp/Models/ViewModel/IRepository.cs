using FastFoodSystem.WebApp.Models.Data;

namespace FastFoodSystem.WebApp.Models.ViewModel
{
    public interface IRepository<T>
    {
        Task<T> GetByIdAsync(string id);
        Task<List<T>> GetAllAsync();
        Task AddAsync(T voucher);
        Task UpdateAsync(T voucher, string id);
        Task DeleteAsync(string id);
    }
}
