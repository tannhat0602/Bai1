using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bai1.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bai1.Controllers
{
    [Authorize]
    [Area("Partner")]
    public class StoresController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StoresController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var store = await _context.Stores
                .Include(s => s.Foods)
                .ThenInclude(f => f.Category)
                .FirstOrDefaultAsync(s => s.OwnerId == userId);

            if (store == null)
            {
                return RedirectToAction(nameof(CreateStore));
            }

            if (store.IsLocked)
            {
                // Lưu thông báo vào TempData và chuyển hướng đến trang thông báo lỗi
                return View("~/Areas/Partner/Views/Stores/LockedStore.cshtml");
            }
            // Nhóm các món ăn theo Category
            var groupedFoods = store.Foods
                                    .GroupBy(f => f.Category)
                                    .ToList();

            ViewBag.GroupedFoods = groupedFoods;

            return View(store);
        }

        public IActionResult CreateStore()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStore(Store store)
        {
            if (ModelState.IsValid)
            {
                store.OwnerId = _userManager.GetUserId(User);
                store.IsApproved = false;
                _context.Stores.Add(store);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(store);
        }
        [HttpGet]
        public IActionResult CreateFood()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");

            var model = new CreateFoodViewModel
            {
                Food = new Food()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFood(CreateFoodViewModel model)
        {
            ModelState.Remove("Food.ImageUrl");
            ModelState.Remove("Food.Store");
            ModelState.Remove("Food.Category");
            ModelState.Remove("Food.StoreId");

            var userId = _userManager.GetUserId(User);
            var store = await _context.Stores.FirstOrDefaultAsync(s => s.OwnerId == userId);

            if (store == null)
            {
                ModelState.AddModelError("", "Bạn chưa có cửa hàng nào.");
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
                return View(model);
            }

            if (model.Image == null || model.Image.Length == 0)
            {
                ModelState.AddModelError("Food.ImageUrl", "Ảnh món ăn là bắt buộc.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
                return View(model);
            }

            var food = model.Food;
            food.StoreId = store.Id;

            if (model.Image != null)
            {
                var fileName = Path.GetFileName(model.Image.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }
                food.ImageUrl = $"/images/{fileName}";
            }

            if (model.NewToppings != null && model.NewToppings.Any())
            {
                foreach (var topping in model.NewToppings)
                {
                    food.Toppings.Add(new Topping
                    {
                        Name = topping.Name,
                        Price = topping.Price ?? 0
                    });
                }
            }

            _context.Foods.Add(food);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> EditStore(int? id)
        {
            if (id == null) return NotFound();

            var store = await _context.Stores.FindAsync(id);
            if (store == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (store.OwnerId != userId) return Forbid();

            return View(store);
        }


        private bool StoreExists(int id)
        {
            return _context.Stores.Any(e => e.Id == id);
        }

        public async Task<IActionResult> EditFood(int? id)
        {
            if (id == null) return NotFound();

            var food = await _context.Foods
                .Include(f => f.Toppings)
                .Include(f => f.Store)
                .FirstOrDefaultAsync(f => f.Id == id);
            if (food == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (food.Store.OwnerId != userId) return Forbid();

            var viewModel = new EditFoodViewModel
            {
                Id = food.Id,
                Name = food.Name,
                Description = food.Description,
                Price = food.Price,
                CategoryId = food.CategoryId,
                ImageUrl = food.ImageUrl,
                Toppings = food.Toppings.Select(t => new ToppingViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    Price = t.Price ?? 0
                }).ToList()
            };

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", food.CategoryId);
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFood(EditFoodViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            var store = await _context.Stores.FirstOrDefaultAsync(s => s.OwnerId == userId);
            if (store == null) return Forbid();

            var foodInDb = await _context.Foods
                .Include(f => f.Toppings)
                .FirstOrDefaultAsync(f => f.Id == model.Id);

            if (foodInDb == null || foodInDb.StoreId != store.Id) return Forbid();

            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", model.CategoryId);
                return View(model);
            }

            foodInDb.Name = model.Name;
            foodInDb.Description = model.Description;
            foodInDb.Price = model.Price;
            foodInDb.CategoryId = model.CategoryId;

            if (model.Image != null)
            {
                var fileName = Path.GetFileName(model.Image.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await model.Image.CopyToAsync(stream);

                foodInDb.ImageUrl = $"/images/{fileName}";
            }

            // Cập nhật topping
            foreach (var topping in model.Toppings)
            {
                if (topping.Id.HasValue)
                {
                    var toppingInDb = foodInDb.Toppings.FirstOrDefault(t => t.Id == topping.Id.Value);
                    if (toppingInDb != null)
                    {
                        if (topping.IsDeleted)
                            _context.Toppings.Remove(toppingInDb);
                        else
                        {
                            toppingInDb.Name = topping.Name;
                            toppingInDb.Price = topping.Price;
                        }
                    }
                }
                else if (!topping.IsDeleted) // topping mới
                {
                    foodInDb.Toppings.Add(new Topping
                    {
                        Name = topping.Name,
                        Price = topping.Price
                    });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> DeleteStore(int? id)
        {
            if (id == null) return NotFound();

            var store = await _context.Stores.Include(s => s.Foods).FirstOrDefaultAsync(s => s.Id == id);
            if (store == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (store.OwnerId != userId) return Forbid();

            return View(store);
        }

        [HttpPost, ActionName("DeleteStore")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStoreConfirmed(int id)
        {
            var store = await _context.Stores.Include(s => s.Foods).FirstOrDefaultAsync(s => s.Id == id);
            if (store == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (store.OwnerId != userId) return Forbid();

            _context.Foods.RemoveRange(store.Foods);
            _context.Stores.Remove(store);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteFood(int? id)
        {
            if (id == null) return NotFound();

            var food = await _context.Foods
                .Include(f => f.Store)
                .Include(f => f.Category)  // Thêm dòng này để load category vào
                .FirstOrDefaultAsync(f => f.Id == id);
            if (food == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (food.Store.OwnerId != userId) return Forbid();

            return View(food);
        }

        [HttpPost, ActionName("DeleteFood")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFoodConfirmed(int id)
        {
            var food = await _context.Foods
                .Include(f => f.Store)
                .Include(f => f.Category)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (food == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (food.Store.OwnerId != userId) return Forbid();

            _context.Foods.Remove(food);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var food = await _context.Foods
                .Include(f => f.Store)
                .Include(f => f.Category)
                .Include(f => f.Toppings) // Nạp các FoodToppings
                .FirstOrDefaultAsync(f => f.Id == id);

            if (food == null)
            {
                return NotFound();
            }

            return View(food);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStore(int id, Store store, IFormFile? image)
        {
            if (id != store.Id) return NotFound();

            ModelState.Remove("OwnerId");
            ModelState.Remove("Owner");
            ModelState.Remove("Foods");

            var existingStore = await _context.Stores.FindAsync(id);
            if (existingStore == null) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    existingStore.Name = store.Name;
                    existingStore.Description = store.Description;
                    existingStore.StreetAddress = store.StreetAddress;
                    existingStore.StreetName = store.StreetName;
                    existingStore.City = store.City;
                    existingStore.District = store.District;
                    existingStore.Phone = store.Phone;

                    if (image != null && image.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                        var uniqueFileName = Guid.NewGuid() + "_" + Path.GetFileName(image.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        existingStore.ImageUrl = "/images/" + uniqueFileName;
                    }

                    _context.Update(existingStore);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Cập nhật thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreExists(store.Id)) return NotFound();
                    else throw;
                }
            }

            return View(store);
        }

        public IActionResult LockedStore()
        {
            return View();  // Trả về view cho trang thông báo cửa hàng bị khóa
        }


        private bool FoodExists(int id)
        {
            return _context.Foods.Any(e => e.Id == id);
        }

        // GET: Partner/Stores/CreateDiscountCode
        public async Task<IActionResult> CreateDiscountCode()
        {
            var userId = _userManager.GetUserId(User);
            var store = await _context.Stores.FirstOrDefaultAsync(s => s.OwnerId == userId);

            if (store == null)
            {
                return RedirectToAction(nameof(CreateStore));
            }

            if (store.IsLocked)
            {
                return View("LockedStore");
            }

            return View(new DiscountCode());
        }

        // POST: Partner/Stores/CreateDiscountCode
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDiscountCode(DiscountCode discountCode)
        {
            var userId = _userManager.GetUserId(User);
            var store = await _context.Stores.FirstOrDefaultAsync(s => s.OwnerId == userId);

            if (store == null)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                discountCode.StoreId = store.Id;
                discountCode.CreatedAt = DateTime.UtcNow;
                ;
                _context.DiscountCodes.Add(discountCode);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tạo mã giảm giá thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(discountCode);
        }


        public IActionResult ToggleOutOfStock(int id)
        {
            var food = _context.Foods.Find(id);
            if (food != null)
            {
                food.IsOutOfStock = !food.IsOutOfStock; // Đảo trạng thái
                _context.SaveChanges();
                TempData["SuccessMessage"] = food.IsOutOfStock
                    ? "Đã đánh dấu món ăn là hết đồ ăn."
                    : "Đã đánh dấu món ăn là có đồ ăn.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> TotalRevenue()
        {
            var userId = _userManager.GetUserId(User);

            // Tìm cửa hàng của Partner hiện tại
            var store = await _context.Stores
                .Include(s => s.Foods)
                .FirstOrDefaultAsync(s => s.OwnerId == userId);

            if (store == null)
            {
                return RedirectToAction(nameof(CreateStore));
            }

            var foodIds = store.Foods.Select(f => f.Id).ToList();
            var now = DateTime.Now.ToUniversalTime();


            // ✅ Đã bỏ điều kiện "Đã giao"
            var query = _context.OrderDetails
                .Where(od => foodIds.Contains(od.FoodId));

            // Tổng doanh thu tuần qua
            var weekRevenue = await query
                .Where(od => od.Order.OrderDate >= now.AddDays(-7))
                .SumAsync(od => od.Quantity * od.Price);

            // Tổng doanh thu tháng qua
            var monthRevenue = await query
                .Where(od => od.Order.OrderDate >= now.AddMonths(-1))
                .SumAsync(od => od.Quantity * od.Price);

            // Tổng doanh thu năm qua
            var yearRevenue = await query
                .Where(od => od.Order.OrderDate >= now.AddYears(-1))
                .SumAsync(od => od.Quantity * od.Price);

            ViewBag.WeekRevenue = weekRevenue;
            ViewBag.MonthRevenue = monthRevenue;
            ViewBag.YearRevenue = yearRevenue;

            return View();
        }




    }
}

