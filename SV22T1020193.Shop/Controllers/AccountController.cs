using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020193.BusinessLayers;
using SV22T1020193.Models.Partner;
using SV22T1020193.Models.Security; // Đảm bảo có namespace này cho DangKyModel
using System.Security.Claims;

namespace SV22T1020193.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountDataService _accountService;

        public AccountController()
        {
            // Khởi tạo service (chuỗi kết nối đã được cấu hình trong AccountDataService)
            _accountService = new AccountDataService();
        }

        #region Đăng nhập
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Vui lòng nhập Email và Mật khẩu.");
                return View();
            }

            var user = await _accountService.AuthorizeCustomerAsync(userName, password);
            if (user == null)
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không chính xác (hoặc tài khoản bị khóa).");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Đăng ký tài khoản Khách hàng

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Register()
        {
            // 1. Load danh sách tỉnh thành cho dropdown trong Form
            ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();

            // 2. PHẢI truyền đúng DangKyModel sang View để tránh lỗi InvalidOperationException
            return View(new DangKyModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(DangKyModel model)
        {
            // 1. Kiểm tra tính hợp lệ của dữ liệu (dựa trên DataAnnotations trong class DangKyModel)
            if (!ModelState.IsValid)
            {
                ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();
                return View(model); // Trả về model hiện tại kèm thông báo lỗi đỏ
            }

            try
            {
                // 2. Ánh xạ (Map) dữ liệu từ DangKyModel sang đối tượng Customer để lưu vào DB
                Customer newCustomer = new Customer()
                {
                    CustomerName = model.CustomerName,
                    ContactName = model.ContactName ?? "",
                    Province = model.Province,
                    Address = model.Address ?? "",
                    Phone = model.Phone ?? "",
                    Email = model.Email,
                    IsLocked = false // Mặc định không khóa khi mới tạo
                };

                // 3. Gọi hàm RegisterCustomerAsync đã viết trong AccountDataService
                int result = await _accountService.RegisterCustomerAsync(newCustomer, model.Password);

                if (result > 0)
                {
                    // Đăng ký thành công -> Về trang Login
                    TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập.";
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "Đăng ký không thành công. Email có thể đã được sử dụng.");
                    ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
                ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();
                return View(model);
            }
        }
        #endregion

        #region Đăng xuất
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        #endregion
    #region Thông tin cá nhân & Đổi mật khẩu

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            // 1. Lấy ID của khách hàng đang đăng nhập từ Cookie
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int customerId = 0;
            int.TryParse(userIdStr, out customerId);

            // 2. Lấy thông tin khách hàng từ DB (Sử dụng PartnerDataService của bạn)
            var customer = await PartnerDataService.GetCustomerAsync(customerId);

            if (customer == null)
                return RedirectToAction("Login", "Account");

            // Load lại danh sách tỉnh thành cho Form sửa
            ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();

            return View(customer);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Profile(Customer data)
        {
            // Cập nhật thông tin cá nhân
            try
            {
                // Ràng buộc ID phải là người đang đăng nhập (tránh việc sửa mã HTML ở Client để hack)
                data.CustomerID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                // Gọi hàm UpdateCustomerAsync từ PartnerDataService (có await vì đây là hàm async)
                bool result = await PartnerDataService.UpdateCustomerAsync(data);

                if (result)
                    TempData["ProfileMessage"] = "Cập nhật thông tin cá nhân thành công!";
                else
                    TempData["ProfileError"] = "Không thể cập nhật thông tin.";
            }
            catch (Exception ex)
            {
                TempData["ProfileError"] = "Lỗi: " + ex.Message;
            }

            return RedirectToAction("Profile");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            // 1. Kiểm tra tính hợp lệ cơ bản
            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword))
                return Json(new { success = false, message = "Vui lòng nhập đầy đủ thông tin mật khẩu." });

            if (newPassword != confirmPassword)
                return Json(new { success = false, message = "Mật khẩu xác nhận không khớp." });

            // 2. Lấy thông tin tài khoản (Email) đang đăng nhập
            // Vì UserName đăng nhập của khách hàng chính là Email
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            // 3. Xác thực mật khẩu cũ xem có đúng không
            // Sử dụng _accountService đã được khởi tạo trong constructor của Controller
            var user = await _accountService.AuthorizeCustomerAsync(userEmail, oldPassword);
            if (user == null)
            {
                return Json(new { success = false, message = "Mật khẩu cũ không chính xác." });
            }

            // 4. Tiến hành đổi mật khẩu mới
            bool isChanged = await _accountService.ChangeCustomerPasswordAsync(userEmail, newPassword);

            if (isChanged)
                return Json(new { success = true, message = "Đổi mật khẩu thành công!" });
            else
                return Json(new { success = false, message = "Lỗi hệ thống, không thể đổi mật khẩu lúc này." });
        }
        #endregion
    }
}