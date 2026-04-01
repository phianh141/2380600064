using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TH2.Models;
using TH2.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cấu hình Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Cấu hình Cookie (Đường dẫn chuyển hướng)
builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

builder.Services.AddRazorPages();

// Đăng ký Repository
builder.Services.AddScoped<IProductRepository, EFProductRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();
builder.Services.AddScoped<IOrderRepository, EFOrderRepository>();

// Thêm dịch vụ Controller với View (Đã xóa 1 dòng trùng lặp)
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>{options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian sống của Session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Hai dòng này bắt buộc phải đi liền nhau
app.UseAuthentication();
app.UseAuthorization();

// --- ĐÃ SỬA LẠI PHẦN MAP ROUTE ---
// Chạy thẳng MapControllerRoute thay vì bọc trong UseEndpoints
app.MapControllerRoute(
    name: "Admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// MapRazorPages giờ đã cùng cấp với MapControllerRoute, sẽ không bị miss Identity nữa
app.MapRazorPages();

app.Run();