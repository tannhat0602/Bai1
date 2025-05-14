using Microsoft.AspNetCore.Identity;

namespace Bai1.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Các thuộc tính hiện có
        public string FullName { get; set; }
        public string? Address { get; set; }
        public string? Age { get; set; }

        // Thuộc tính mới để khóa tài khoản
        public bool IsLocked { get; set; } = false;

        // Thuộc tính liên kết với cửa hàng (Store)
        public Store Store { get; set; }
        public bool IsDarkMode { get; set; } = false;

    }

}
