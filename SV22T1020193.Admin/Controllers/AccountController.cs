using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020193.Models.Security;
using SV22T1020193.BusinessLayers;

namespace SV22T1020193.Admin.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly AccountDataService _accountService;

        // Tiêm AccountDataService thông qua Constructor
        public AccountController(AccountDataService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password)
        {
            ViewBag.Username = username;

            // 1. Kiểm tra đầu vào rỗng
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Vui lòng nhập email và mật khẩu");
                return View();
            }

            // 2. Mã hóa mật khẩu người dùng nhập
            string hashedPassword = CryptHelper.HashMD5(password);

            // 3. Gọi Database kiểm tra tài khoản thực tế thay vì giả lập
            var userAccount = await _accountService.AuthorizeEmployeeAsync(username, hashedPassword);

            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại. Kiểm tra lại thông tin.");
                return View();
            }

            // 4. Thiết lập thông tin người dùng vào chứng nhận (Cookie)
            var userData = new WebUserData()
            {
                UserId = userAccount.UserId,
                UserName = userAccount.UserName,
                DisplayName = userAccount.DisplayName,
                Email = userAccount.Email,
                Photo = string.IsNullOrEmpty(userAccount.Photo) ? "nophoto.png" : userAccount.Photo,
                Roles = string.IsNullOrEmpty(userAccount.RoleNames)
                            ? new List<string>()
                            : userAccount.RoleNames.Split(',').ToList()
            };

            // Tạo ra giấy chứng nhận (ClaimPrincipal)
            var principal = userData.CreatePrincipal();

            // Trao Giấy chứng nhận cho phía client
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Mật khẩu xác nhận không khớp");
                return View();
            }

            // Lấy thông tin user hiện tại đang đăng nhập
            var userData = User.GetUserData();
            if (userData == null) return RedirectToAction("Login");

            string hashedOldPassword = CryptHelper.HashMD5(oldPassword);
            string hashedNewPassword = CryptHelper.HashMD5(newPassword);

            // Kiểm tra mật khẩu cũ có đúng không
            var checkAuth = await _accountService.AuthorizeEmployeeAsync(userData.UserName, hashedOldPassword);
            if (checkAuth == null)
            {
                ModelState.AddModelError("", "Mật khẩu cũ không chính xác");
                return View();
            }

            // Thực hiện đổi sang mật khẩu mới
            bool isSuccess = await _accountService.ChangeEmployeePasswordAsync(userData.UserName, hashedNewPassword);

            if (isSuccess)
            {
                // Sau khi đổi pass thành công, bắt đăng nhập lại để làm mới cookie
                await HttpContext.SignOutAsync();
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Đổi mật khẩu thất bại. Vui lòng thử lại sau.");
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}