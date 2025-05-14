// Models/ViewModels/ApplicationUserViewModel.cs
namespace Bai1.Models.ViewModels
{
    public class ApplicationUserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool IsLocked { get; set; }
        public string RolesString { get; set; }  // Vai trò dưới dạng chuỗi
    }
}
