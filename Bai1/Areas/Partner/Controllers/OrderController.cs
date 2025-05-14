using Bai1.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bai1.Areas.Partner.Controllers
{
    [Area("Partner")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách đơn hàng
        public IActionResult Index()
        {
            var orders = _context.Orders.ToList();
            return View(orders);
        }

        // Thêm action Confirm (hiển thị danh sách đơn hàng chưa xác nhận)
        [HttpGet]
        // Action Confirm: hiển thị đơn chưa xác nhận và chưa hủy
        public IActionResult Confirm()
        {
            var orders = _context.Orders
                                 .Where(o => o.Status != "Đã xác nhận" && o.Status != "Đã hủy")
                                 .ToList();

            return View(orders);
        }


        // Xác nhận đơn hàng
        [HttpPost]
        public IActionResult ConfirmOrder(int orderId)
        {
            var order = _context.Orders.Find(orderId);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = "Đã xác nhận"; // Cập nhật trạng thái đơn hàng
            _context.SaveChanges();

            return RedirectToAction("Confirm"); // Sau khi xác nhận xong, quay lại trang Confirm
        }
        // Hủy đơn hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder(int orderId)
        {
            var order = _context.Orders.Find(orderId);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = "Đã hủy"; // Cập nhật trạng thái đơn hàng
            _context.SaveChanges();

            return RedirectToAction("Confirm"); // Sau khi hủy xong, quay lại danh sách cần xác nhận
        }


    }
}
