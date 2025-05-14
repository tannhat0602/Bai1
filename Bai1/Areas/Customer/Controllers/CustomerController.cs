using Bai1.Models;
using Bai1.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bai1.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Index()
        {
            // Trang Index cho Customer
            return View();
        }
        [HttpGet]
        public IActionResult RegisterPartner()
        {
            var user = _userManager.GetUserAsync(User).Result; // Lấy người dùng đang đăng nhập
            if (user == null)
            {
                // Người dùng không đăng nhập, điều hướng về trang đăng nhập
                return RedirectToAction("Login", "Account");
            }

            // Kiểm tra xem người dùng đã có yêu cầu đăng ký chưa và trạng thái yêu cầu
            var existingRequest = _context.PartnerRequests
                .FirstOrDefault(pr => pr.UserId == user.Id && pr.IsApproved == false); // Kiểm tra yêu cầu chưa duyệt

            if (existingRequest != null)
            {
                // Nếu yêu cầu đã tồn tại và chưa duyệt, điều hướng về trang Index
                TempData["SuccessMessage"] = "Bạn đã gửi yêu cầu đăng ký đối tác và đang chờ duyệt.";
                return RedirectToAction("Index", "Customer");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterPartner(PartnerRequest partnerRequest)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);  // Lấy người dùng đã đăng nhập từ UserManager
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Không tìm thấy người dùng.");
                    return View(partnerRequest);  // Trả về form với lỗi
                }

                // Kiểm tra nếu người dùng đã có vai trò là "Partner"
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Partner"))
                {
                    // Nếu người dùng đã là đối tác, chuyển hướng về trang Index
                    return RedirectToAction("Index", "Customer");  // Redirect đến trang Index trong Area Customer
                }

                // Kiểm tra nếu người dùng đã đăng ký yêu cầu trước đó và yêu cầu chưa được duyệt
                var existingRequest = await _context.PartnerRequests
                    .FirstOrDefaultAsync(pr => pr.UserId == user.Id && pr.IsApproved == false);
                if (existingRequest != null)
                {
                    // Nếu đã có yêu cầu và chưa duyệt, chuyển hướng về trang Index (chờ duyệt)
                    TempData["SuccessMessage"] = "Bạn đã gửi yêu cầu đăng ký đối tác và đang chờ duyệt.";
                    return RedirectToAction("Index", "Customer");  // Redirect đến trang Index trong Area Customer
                }

                // Gán thông tin của người dùng vào PartnerRequest
                partnerRequest.UserId = user.Id;  // Sử dụng Id của người dùng thay vì UserName
                partnerRequest.User = user;  // Gán đối tượng User
                partnerRequest.IsApproved = false;  // Đặt là false khi yêu cầu đang chờ duyệt

                // Lưu yêu cầu đăng ký vào cơ sở dữ liệu
                _context.Add(partnerRequest);
                await _context.SaveChangesAsync();

                // Thêm thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Đăng ký đối tác thành công! Vui lòng đợi admin duyệt.";

                // Chuyển hướng người dùng về trang Customer sau khi đăng ký thành công
                return RedirectToAction("Index", "Customer", new { area = "Customer" });
            }

            // Nếu form không hợp lệ, trả về form với các lỗi
            return View(partnerRequest);

        }

        public async Task<IActionResult> MyRequest()
        {
            var user = await _userManager.GetUserAsync(User);
            var request = await _context.PartnerRequests
                                .FirstOrDefaultAsync(r => r.UserId == user.Id);

            if (request == null)
            {
                ViewBag.Message = "Bạn chưa có yêu cầu nào.";
                return View();
            }

            if (request.IsApproved == true)
            {
                ViewBag.Message = "Yêu cầu của bạn đã được duyệt!";
            }
            else
            {
                ViewBag.Message = "Yêu cầu của bạn đang chờ duyệt hoặc đã bị từ chối.";
            }

            return View(request);
        }

    }
}
