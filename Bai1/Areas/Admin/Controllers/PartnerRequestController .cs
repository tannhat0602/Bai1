using Bai1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Bai1.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Bai1.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PartnerRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public PartnerRequestController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;

        }

        // GET: PartnerRequest
        public async Task<IActionResult> Index()
        {
            var partnerRequests = await _context.PartnerRequests
                                                .Where(r => !r.IsApproved) // Chỉ lấy các yêu cầu chưa được duyệt
                                                .ToListAsync();
            return View(partnerRequests);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var request = await _context.PartnerRequests.FirstOrDefaultAsync(r => r.Id == id);
            if (request == null)
            {
                return NotFound();  // Nếu không tìm thấy yêu cầu
            }

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return NotFound();  // Nếu không tìm thấy người dùng
            }

            // Kiểm tra xem người dùng đã có vai trò "Partner" chưa
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Partner"))
            {
                TempData["ErrorMessage"] = "Người dùng đã có vai trò là đối tác.";
                return BadRequest("Người dùng đã có vai trò là đối tác.");
            }

            // Cập nhật trạng thái yêu cầu và phê duyệt
            request.IsApproved = true;
            request.ApprovalDate = DateTime.Now.ToUniversalTime(); ;
            _context.Update(request);

            // Thêm vai trò "Partner" cho người dùng
            var result = await _userManager.AddToRoleAsync(user, "Partner");
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "Không thể thêm vai trò 'Partner' cho người dùng.";
                return BadRequest("Không thể thêm vai trò 'Partner' cho người dùng.");
            }

            // Kiểm tra nếu người dùng đã có cửa hàng thì cập nhật, chưa có thì tạo mới
            var existingStore = await _context.Stores
                                        .FirstOrDefaultAsync(s => s.OwnerId == request.UserId);
            if (existingStore == null)
            {
                var newStore = new Store
                {
                    Name = request.BusinessName,
                    Description = request.Description,
                    OwnerId = request.UserId,
                    StreetName = request.StreetName,
                    City = request.City,
                    District = request.District,
                    StreetAddress = request.StreetAddress,
                    Phone = request.BusinessPhone,
                    IsApproved = true  // <-- Gán luôn trạng thái Approved khi tạo mới
                };

                _context.Stores.Add(newStore);
            }
            else
            {
                // Nếu có rồi thì cập nhật trạng thái IsApproved
                existingStore.IsApproved = true;
                _context.Stores.Update(existingStore);
            }

            await _context.SaveChangesAsync();  // Lưu toàn bộ thay đổi

            TempData["SuccessMessage"] = "Phê duyệt thành công!";
            return RedirectToAction("Index", "PartnerRequest", new { area = "Admin" });
        }



        // GET: PartnerRequest/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var partnerRequest = await _context.PartnerRequests.FindAsync(id);
            if (partnerRequest == null)
            {
                return NotFound();
            }

            // Thay đổi trạng thái duyệt
            _context.PartnerRequests.Remove(partnerRequest);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Yêu cầu đối tác của bạn đã bị từ chối! Bạn có thể đăng ký lại.";

            // Cập nhật lại trạng thái vào cơ sở dữ liệu


            return RedirectToAction("Index", "PartnerRequest", new { area = "Admin" });
        }
    }

}

