using Microsoft.AspNetCore.Authentication.Cookies; // BỔ SUNG USING NÀY
using SV22T1020193.BusinessLayers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddMvcOptions(option => option.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

// 1. Cấu hình Session (Cần thiết cho Giỏ hàng)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 2. BỔ SUNG: Cấu hình Authentication (Xác thực bằng Cookie)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.Cookie.Name = "ShopAuthenticationCookie"; // Tên cookie
        option.LoginPath = "/Account/Login";             // Đường dẫn tự động chuyển hướng khi cần đăng nhập
        option.AccessDeniedPath = "/Account/AccessDenied";
        option.ExpireTimeSpan = TimeSpan.FromDays(30);   // Thời gian sống của phiên đăng nhập (vd: 30 ngày)
    });

var app = builder.Build();

// Khởi tạo cấu hình cho BusinessLayer
string connectionString = builder.Configuration.GetConnectionString("LiteCommerceDB")
    ?? throw new InvalidOperationException("ConnectionString 'LiteCommerceDB' not found.");

SV22T1020193.BusinessLayers.Configuration.Initialize(connectionString);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Kích hoạt Session cho Giỏ hàng

// 3. BỔ SUNG: Kích hoạt Authentication
app.UseAuthentication(); // BẮT BUỘC phải nằm trước UseAuthorization()

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();