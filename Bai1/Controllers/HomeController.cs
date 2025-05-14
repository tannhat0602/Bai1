using Bai1.Models;
using Bai1.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IFoodRepository _foodRepository;
    private readonly ApplicationDbContext _context;


    public HomeController(ILogger<HomeController> logger, IFoodRepository foodRepository, ApplicationDbContext context)
    {
        _logger = logger;
        _foodRepository = foodRepository;
        _context = context;
    }

    public async Task<IActionResult> Index(string searchQuery, string bannerUrl)
    {
        var stores = await _context.Stores
        .Where(s => s.IsApproved)
        .Include(s => s.Foods)
        .ToListAsync();

        // Nếu có từ khóa tìm kiếm, lọc danh sách món ăn
        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            stores = stores
                .Where(f => f.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // Đọc thông tin banner từ tệp
        var bannerConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/banner.txt");
        if (System.IO.File.Exists(bannerConfigPath))
        {
            var bannerUrls = System.IO.File.ReadAllText(bannerConfigPath);
            ViewData["BannerUrl"] = bannerUrls;
        }
        else
        {
            // Banner mặc định nếu không có tệp banner.txt
            ViewData["BannerUrl"] = "/images/default-banner.jpg";
        }

        // Truyền danh sách món ăn vào view
        return View(stores);
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    public async Task<IActionResult> Search(string query)
    {
        var foods = await _foodRepository.GetAllAsync();

        // Lọc theo tên món ăn
        if (!string.IsNullOrWhiteSpace(query))
        {
            foods = foods
                .Where(f => !string.IsNullOrEmpty(f.Name) &&
                            f.Name.Contains(query, StringComparison.OrdinalIgnoreCase) &&
                            f.Store != null)
                .ToList();
        }

        // Lấy danh sách các Store chứa món ăn đó (không trùng lặp)
        var stores = foods
            .Where(f => f.Store != null)
            .Select(f => f.Store)
            .Distinct()
            .ToList();

        ViewBag.Query = query;
        return View("Search", stores); // Trả về view với model là List<Store>
    }




}
