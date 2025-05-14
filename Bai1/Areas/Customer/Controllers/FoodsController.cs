using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bai1.Models;
using Microsoft.AspNetCore.Identity;

namespace Bai1.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class FoodsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FoodsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Customer/Foods
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Foods.Include(f => f.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Customer/Foods/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Lấy thông tin món ăn kèm danh mục và ảnh
            var food = await _context.Foods
                .Include(f => f.Category)
                .Include(f => f.Images)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (food == null)
            {
                return NotFound();
            }

            // Lấy ID người dùng hiện tại (đã đăng nhập)
            var userId = _userManager.GetUserId(User);

            // Kiểm tra xem người dùng đã đặt món ăn này trong đơn hàng "Completed"
            bool hasOrdered = await _context.Orders
                .Where(o => o.UserId == userId && o.Status == "Đã xác nhận")
                .SelectMany(o => o.OrderDetails)
                .AnyAsync(od => od.FoodId == id);

            // Debug check for order status
            if (!hasOrdered)
            {
                Console.WriteLine("User has not ordered this food.");
            }

            // Truyền biến kiểm tra vào ViewBag
            ViewBag.CanReview = hasOrdered;

            // Lấy các đánh giá đã có cho món ăn này, bao gồm thông tin người đánh giá
            var reviews = await _context.FoodReviews
                .Include(r => r.User)
                .Where(r => r.FoodId == id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            ViewBag.Reviews = reviews;

            return View(food);
        }


        // POST: Customer/Foods/AddReview
        [HttpPost]
        public IActionResult AddReview(FoodReview review)
        {
            if (ModelState.IsValid)
            {
                review.CreatedAt = DateTime.UtcNow;
                review.UserId = _userManager.GetUserId(User); // Lấy ID user hiện tại
                _context.FoodReviews.Add(review);
                _context.SaveChanges();

                TempData["Success"] = "Đánh giá của bạn đã được gửi thành công!";
                return RedirectToAction("Details", new { id = review.FoodId });
            }

            // Nếu ModelState không hợp lệ, load lại trang với dữ liệu
            var food = _context.Foods
                .Include(f => f.Reviews)
                .FirstOrDefault(f => f.Id == review.FoodId);

            ViewBag.Reviews = food.Reviews;
            return View("Details", food);
        }

    }
}
