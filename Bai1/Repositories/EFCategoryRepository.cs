using Bai1.Models;
using Microsoft.EntityFrameworkCore;

namespace Bai1.Repositories
{
    public class EFCategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public EFCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            // return await _context.Products.ToListAsync(); 
            return await _context.Categories.Include(p => p.Foods) // Include thông tin về category 
        .ToListAsync();

        }

        public async Task<Category> GetByIdAsync(int id)
        {
            // return await _context.Products.FindAsync(id); 
            // lấy thông tin kèm theo category 
            return await _context.Categories
                .Include(c => c.Foods)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Category categories)
        {
            _context.Categories.Add(categories);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category categories)
        {
            _context.Categories.Update(categories);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var categories = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(categories);
            await _context.SaveChangesAsync();
        }

    }
}
