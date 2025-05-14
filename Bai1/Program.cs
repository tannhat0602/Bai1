    using Bai1.Hubs;
    using Bai1.Models;
    using Bai1.Repositories;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    var builder = WebApplication.CreateBuilder(args);

    // Cấu hình cookie cho Identity
    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Identity/Account/Login";  // Đường dẫn trang đăng nhập
        options.LogoutPath = "/Identity/Account/Logout";  // Đường dẫn trang đăng xuất
        options.AccessDeniedPath = "/Identity/Account/AccessDenied";  // Đường dẫn khi không có quyền truy cập
        options.ReturnUrlParameter = "returnUrl";  // Tham số URL trả về
    });
    builder.Services.AddHttpClient();

    // Thêm các dịch vụ vào container
    builder.Services.AddScoped<IFoodRepository, EFFoodRepository>();
    builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();

    // Cấu hình bộ nhớ phân tán và session
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);  // Thời gian hết hạn session
        options.Cookie.HttpOnly = true;  // Đảm bảo cookie chỉ có thể truy cập thông qua HTTP
        options.Cookie.IsEssential = true;  // Đảm bảo session cookie luôn có mặt
    });

    // Cấu hình dịch vụ MVC
    builder.Services.AddControllersWithViews();

    // Cấu hình DbContext cho ApplicationDbContext
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Cấu hình Identity
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;  // Tùy chỉnh theo yêu cầu
    })
        .AddRoles<IdentityRole>()  // Thêm cấu hình role
        .AddDefaultTokenProviders()  // Cung cấp các token để xác thực
        .AddDefaultUI()  // Cung cấp các giao diện mặc định cho Identity
        .AddEntityFrameworkStores<ApplicationDbContext>();  // Cấu hình Entity Framework để lưu trữ dữ liệu Identity

    builder.Services.AddRazorPages();  // Thêm hỗ trợ cho Razor Pages
    builder.Services.AddSignalR();  // Thêm SignalR cho chat box

    var app = builder.Build();

    // Cấu hình pipeline HTTP
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");  // Xử lý lỗi trong môi trường không phải development
        app.UseHsts();  // Thêm HTTP Strict Transport Security
    }

    app.UseHttpsRedirection();  // Chuyển hướng HTTP sang HTTPS
    app.UseStaticFiles();  // Cung cấp các file tĩnh như ảnh, CSS, JS

    app.UseSession();  // Kích hoạt Session
    app.MapHub<ChatHub>("/chathub");  // Cấu hình SignalR hub

    app.UseRouting();  // Bật routing để xử lý các yêu cầu

    app.UseAuthentication();  // Bật xác thực người dùng
    app.UseAuthorization();  // Bật phân quyền người dùng

// Cấu hình Endpoints
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");  // Cấu hình routing cho Area

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");  // Cấu hình routing mặc định


app.MapRazorPages();  // Bật hỗ trợ Razor Pages

    // Chạy ứng dụng
    app.Run();

