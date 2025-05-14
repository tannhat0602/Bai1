using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace Bai1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public AdminController(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            return View("~/Areas/Admin/Views/Admin/Index.cshtml");
        }
        public IActionResult UploadBanner()
        {
            return View("~/Areas/Admin/Views/Admin/UploadBanner.cshtml");
        }
        public IActionResult PartnerStores()
        {
            // Trả về view PartnerStores.cshtml
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadBanner(IFormFile bannerImage)
        {
            if (bannerImage != null && bannerImage.Length > 0)
            {
                var fileName = Path.GetFileName(bannerImage.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await bannerImage.CopyToAsync(stream);
                }

                // Lưu đường dẫn vào file config nhỏ
                var bannerConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/banner.txt");
                System.IO.File.WriteAllText(bannerConfigPath, "/images/" + fileName);
            }

            return RedirectToAction("UploadBanner");
        }


    }
}
