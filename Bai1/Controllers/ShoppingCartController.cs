using Bai1.Extensions;
using Bai1.Models;
using Bai1.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bai1.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IFoodRepository _foodRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoppingCartController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager, IFoodRepository foodRepository)
        {
            _foodRepository = foodRepository;
            _context = context;
            _userManager = userManager;
        }

        // Hiển thị giỏ hàng
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var items = _context.UserCartItems
                .Where(c => c.UserId == user.Id && c.AddedAt > DateTime.UtcNow.AddDays(-1))
                .Select(c => new CartItem
                {
                    ProductId = c.FoodId,
                    Name = c.Food.Name,
                    Price = c.Food.Price,
                    Quantity = c.Quantity
                }).ToList();

            var cart = new ShoppingCart { Items = items };
            return View(cart);
        }


        // Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public async Task<JsonResult> AddToCart(int productId, int quantity)
        {
            var user = await _userManager.GetUserAsync(User);
            var existingItem = _context.UserCartItems
                .FirstOrDefault(ci => ci.UserId == user.Id && ci.FoodId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var newItem = new UserCartItem
                {
                    UserId = user.Id,
                    FoodId = productId,
                    Quantity = quantity,
                    AddedAt = DateTime.UtcNow
                };
                _context.UserCartItems.Add(newItem);
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Đã thêm vào giỏ hàng." });
        }


        // Xóa sản phẩm khỏi giỏ hàng
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");

            if (cart != null)
            {
                cart.RemoveItem(productId);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            return RedirectToAction("Index");
        }

        // Giao diện thanh toán
        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            var checkoutViewModel = new CheckoutViewModel
            {
                Order = new Order(),
                Cart = cart
            };
            return View(checkoutViewModel);
        }

        // Xử lý thanh toán
        [HttpPost]
        public async Task<IActionResult> Checkout(Order order, string PaymentMethod)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");

            if (cart == null || !cart.Items.Any())
            {
                ViewBag.Message = "Giỏ hàng của bạn đang trống!";
                return View(new CheckoutViewModel
                {
                    Order = new Order(),
                    Cart = new ShoppingCart()
                });
            }

            var user = await _userManager.GetUserAsync(User);
            order.UserId = user.Id;
            order.OrderDate = DateTime.UtcNow;
            order.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);
            order.PaymentMethod = PaymentMethod;

            order.OrderDetails = cart.Items.Select(i => new OrderDetail
            {
                FoodId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList();

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // ✅ Xoá giỏ hàng khỏi DB sau khi đặt hàng
            _context.UserCartItems.RemoveRange(
                _context.UserCartItems.Where(c => c.UserId == user.Id)
            );
            await _context.SaveChangesAsync();

            return RedirectToAction("OrderCompleted", new { orderId = order.Id });
        }

        // Hiển thị đơn hàng sau khi hoàn tất
        public IActionResult OrderCompleted(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View(orderId);
        }

        // Hiển thị lịch sử đơn hàng của người dùng
        [Authorize]
        public async Task<IActionResult> MyOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            var orders = _context.Orders
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            var newOrder = orders.FirstOrDefault(); // đơn mới nhất
            var oldOrders = orders.Skip(1).ToList(); // các đơn cũ

            var model = new Tuple<Order, List<Order>>(newOrder, oldOrders);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> HandleSelectedItems(List<int> selectedProductIds, string actionType)
        {
            var user = await _userManager.GetUserAsync(User);

            if (selectedProductIds == null || !selectedProductIds.Any())
            {
                TempData["Message"] = "Vui lòng chọn ít nhất một món!";
                return RedirectToAction("Index");
            }

            if (actionType == "remove")
            {
                var itemsToRemove = _context.UserCartItems
                    .Where(c => c.UserId == user.Id && selectedProductIds.Contains(c.FoodId));
                _context.UserCartItems.RemoveRange(itemsToRemove);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Đã xóa món đã chọn.";
                return RedirectToAction("Index");
            }
            else if (actionType == "checkout")
            {
                // Lưu các món được chọn vào Session như một giỏ hàng tạm thời để Checkout
                var items = _context.UserCartItems
                    .Where(c => c.UserId == user.Id && selectedProductIds.Contains(c.FoodId))
                    .Select(c => new CartItem
                    {
                        ProductId = c.FoodId,
                        Name = c.Food.Name,
                        Price = c.Food.Price,
                        Quantity = c.Quantity
                    }).ToList();

                var selectedCart = new ShoppingCart { Items = items };
                HttpContext.Session.SetObjectAsJson("Cart", selectedCart);

                return RedirectToAction("Checkout");
            }

            return RedirectToAction("Index");
        }


        // Hàm phụ trợ (không được dùng trực tiếp)
        private async Task<Food> GetProductFromDatabase(int productId)
        {
            return await _foodRepository.GetByIdAsync(productId);
        }
    }
}
