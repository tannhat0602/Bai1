using Bai1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class StoreController : Controller
{
    private readonly ApplicationDbContext _context;

    public StoreController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Details(int id)
    {
        var store = await _context.Stores
            .Include(s => s.Foods) // Nếu có
             .ThenInclude(f => f.Category)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (store == null)
        {
            return NotFound();
        }

        // Nhóm các món ăn theo Category
        var groupedFoods = store.Foods
                                .GroupBy(f => f.Category)
                                .ToList();

        ViewBag.GroupedFoods = groupedFoods;

        return View(store);
    }

}
