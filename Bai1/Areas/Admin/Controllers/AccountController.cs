using Bai1.Models;
using Bai1.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Bai1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy tất cả người dùng từ UserManager
            var users = await _context.Users.ToListAsync();

            // Lọc những người không phải admin, dựa trên RolesString trong ViewModel
            var userViewModels = new List<ApplicationUserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                // Chỉ lấy người dùng không có vai trò "Admin"
                if (!roles.Contains("Admin"))
                {
                    userViewModels.Add(new ApplicationUserViewModel
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        IsLocked = user.IsLocked,
                        RolesString = string.Join(", ", roles)
                    });
                }
            }

            // Trả về danh sách người dùng không phải admin
            return View(userViewModels);
        }


        // POST: Admin/Account/LockUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Kiểm tra lại vai trò của người dùng và đảm bảo rằng trạng thái IsLocked có thể thay đổi
            var roles = await _userManager.GetRolesAsync(user);

            // In ra log để kiểm tra các roles của người dùng
            Console.WriteLine("Roles: " + string.Join(", ", roles));

            // Đảm bảo rằng tài khoản có thể bị khóa
            if (roles.Contains("Customer") || roles.Contains("Partner"))
            {
                user.IsLocked = true;
                _context.Update(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Tài khoản {user.UserName} đã bị khóa thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể khóa tài khoản này!";
            }

            return RedirectToAction(nameof(Index));
        }



        // POST: Admin/Account/UnlockUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlockUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsLocked = false;
            _context.Update(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tài khoản đã được mở khóa thành công!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Account/DeleteUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.Users
                .Include(u => u.Store)
                    .ThenInclude(s => s.Foods)  // Đảm bảo xóa toàn bộ dữ liệu liên quan trong cửa hàng
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            // Lấy danh sách vai trò của người dùng
            var roles = await _userManager.GetRolesAsync(user);

            // Nếu user có vai trò "Partner" và có cửa hàng
            if (roles.Contains("Partner") && user.Store != null)
            {
                // Xóa các thực phẩm trong cửa hàng
                _context.Foods.RemoveRange(user.Store.Foods);

                // Xóa cửa hàng
                _context.Stores.Remove(user.Store);

                // Lưu các thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();
            }

            // Xóa các yêu cầu đối tác liên quan đến người dùng trong bảng PartnerRequests
            var partnerRequests = await _context.PartnerRequests
                .Where(pr => pr.UserId == user.Id)
                .ToListAsync();
            

            // Lưu các thay đổi vào cơ sở dữ liệu sau khi xóa dữ liệu trong PartnerRequests
            _context.Update(user);
            await _context.SaveChangesAsync();


            // Xóa tài khoản người dùng
            await _userManager.DeleteAsync(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tài khoản và dữ liệu liên quan đã bị xóa thành công!";
            return RedirectToAction(nameof(Index));
        }



    }
}
