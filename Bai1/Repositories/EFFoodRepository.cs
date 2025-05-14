using Bai1.Models;
using Microsoft.EntityFrameworkCore;

namespace Bai1.Repositories
{
    public class EFFoodRepository : IFoodRepository
    {
        private readonly ApplicationDbContext _context;

        public EFFoodRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Food>> GetAllAsync()
        {
            var foods = await _context.Foods
                .Include(p => p.Category) // Include thông tin về category
                .Include(p => p.Store)    // Include thông tin về store
                .ToListAsync();

            foreach (var food in foods)
            {
                // Kiểm tra và xử lý nếu các thuộc tính có thể bị null
                food.Category = food.Category ?? new Category();  // Đảm bảo Category không null
                food.Store = food.Store ?? new Store();  // Đảm bảo Store không null
            }

            return foods;
        }


        public async Task<Food> GetByIdAsync(int id)
        {
            // return await _context.Products.FindAsync(id); 
            // lấy thông tin kèm theo category 
            return await _context.Foods.Include(p =>p.Category).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Food food)
        {
            _context.Foods.Add(food);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Food food)
        {
            _context.Foods.Update(food);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var food = await _context.Foods.FindAsync(id);
            _context.Foods.Remove(food);
            await _context.SaveChangesAsync();
        }

    }
}
