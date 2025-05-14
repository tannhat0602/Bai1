using Bai1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bai1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]  // Chỉ Admin mới vào được

    public class PartnerStoresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PartnerStoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/StoresManagement
        public async Task<IActionResult> Index()
        {
            var stores = await _context.Stores.Include(s => s.Owner).ToListAsync();
            return View(stores);
        }

        [HttpPost]

        public async Task<IActionResult> Lock(int id)
        {
            var store = await _context.Stores.FindAsync(id);
            if (store == null)
            {
                TempData["ErrorMessage"] = "Cửa hàng không tồn tại.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                store.IsLocked = true;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cửa hàng đã được khóa thành công.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Có lỗi xảy ra: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        public async Task<IActionResult> Unlock(int id)
        {
            var store = await _context.Stores.FindAsync(id);
            if (store == null) return NotFound();

            store.IsLocked = false;
            store.UnlockDate = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Mở khoá cửa hàng thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}
