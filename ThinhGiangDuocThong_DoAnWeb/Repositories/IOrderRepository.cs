using ThinhGiangDuocThong_DoAnWeb.Models;

namespace ThinhGiangDuocThong_DoAnWeb.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(int id);
        Task<IEnumerable<Order>> GetByUserIdAsync(string userId);
        Task<Order?> GetByIdWithDetailsAsync(int id);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(int id);
    }
} 