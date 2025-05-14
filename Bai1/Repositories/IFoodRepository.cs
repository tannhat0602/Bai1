namespace Bai1.Repositories;
using System.Collections.Generic;
using Bai1.Models; // Thay thế bằng namespace thực tế của bạn
using System.Threading.Tasks;
public interface IFoodRepository
{
    Task<IEnumerable<Food>> GetAllAsync();
    Task<Food> GetByIdAsync(int id);
    Task AddAsync(Food product);
    Task UpdateAsync(Food product);
    Task DeleteAsync(int id);
}



