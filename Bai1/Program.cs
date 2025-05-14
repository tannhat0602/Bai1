using Bai1.Hubs;
using Bai1.Models;
using Bai1.Repositories;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình cookie cho Identity
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.ReturnUrlParameter = "returnUrl";
});

builder.Services.AddHttpClient();
builder.Services.AddScoped<IFoodRepository, EFFoodRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\keys"))  // Đảm bảo thư mục này tồn tại và có quyền truy cập
    .SetApplicationName("MyApp");  // Đặt tên ứng dụng để phân biệt các khóa bảo vệ
builder.Services.AddControllersWithViews();

// PostgreSQL cho Render
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cấu hình Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddDefaultTokenProviders()
.AddDefaultUI()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();
builder.Services.AddSignalR();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Render đã dùng HTTPS, không cần redirect
                   // KHÔNG bật UseHttpsRedirection để tránh lỗi cổng HTTPS
    app.UseHttpsRedirection(); // 🔁 Đừng bỏ dòng này

}

app.UseStaticFiles();

app.UseSession();
app.MapHub<ChatHub>("/chathub");

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";

app.Run();
