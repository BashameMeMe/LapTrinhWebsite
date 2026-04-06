using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020193.BusinessLayers;
using SV22T1020193.Models.Common;
using SV22T1020193.Models.HR;
using System.Security.Cryptography;
using System.Text;

namespace SV22T1020193.Admin.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private const string EMPLOYEE_SEARCH = "EmployeeSearch";

        // ================= INDEX =================
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(EMPLOYEE_SEARCH);

            if (input == null)
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = 5,
                    SearchValue = ""
                };
            }

            return View(input);
        }

        // ================= SEARCH =================
        public async Task<IActionResult> Search(PaginationSearchInput input)
        {
            var result = await HRDataService.ListEmployeesAsync(input);
            ApplicationContext.SetSessionData(EMPLOYEE_SEARCH, input);

            return PartialView("Search", result);
        }

        // ================= CREATE =================
        public IActionResult Create()
        {
            ViewBag.Title = "Thêm nhân viên";

            return View("Edit", new Employee()
            {
                EmployeeID = 0,
                IsWorking = true
            });
        }

        // ================= EDIT =================
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Cập nhật nhân viên";

            var employee = await HRDataService.GetEmployeeAsync(id);
            if (employee == null)
                return RedirectToAction("Index");

            return View("Edit", employee);
        }

        // ================= SAVE =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(Employee data)
        {
            ViewBag.Title = data.EmployeeID == 0
                ? "Thêm nhân viên"
                : "Cập nhật nhân viên";

            // Validate
            if (string.IsNullOrWhiteSpace(data.FullName))
                ModelState.AddModelError(nameof(data.FullName), "Nhập họ tên");

            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Nhập email");

            if (await HRDataService.ValidateEmployeeEmailAsync(data.Email, data.EmployeeID))
                ModelState.AddModelError(nameof(data.Email), "Email đã tồn tại");

            if (!ModelState.IsValid)
                return View("Edit", data);

            // Chuẩn hóa dữ liệu
            data.Address ??= "";
            data.Phone ??= "";
            data.Photo ??= "";
            data.RoleNames ??= "";

            // Password mặc định
            if (data.EmployeeID == 0)
                data.Password = "123456"; // 👉 nên hash ở Repository

            // Lưu DB
            if (data.EmployeeID == 0)
                await HRDataService.AddEmployeeAsync(data);
            else
                await HRDataService.UpdateEmployeeAsync(data);

            return RedirectToAction("Index");
        }

        // ================= DELETE (GET) =================
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await HRDataService.GetEmployeeAsync(id);
            if (employee == null)
                return RedirectToAction("Index");

            ViewBag.AllowDelete = !await HRDataService.IsUsedEmployeeAsync(id);
            return View(employee);
        }

        // ================= DELETE (POST) =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection form)
        {
            if (await HRDataService.IsUsedEmployeeAsync(id))
            {
                TempData["Error"] = "Nhân viên đang được sử dụng!";
                return RedirectToAction("Delete", new { id });
            }

            await HRDataService.DeleteEmployeeAsync(id);
            return RedirectToAction("Index");
        }

        // ================= CHANGE PASSWORD =================
        [HttpGet]
        public async Task<IActionResult> ChangePassword(int id)
        {
            var employee = await HRDataService.GetEmployeeAsync(id);
            if (employee == null)
                return RedirectToAction("Index");

            ViewBag.Employee = employee;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(int id, string newPassword, string confirmPassword)
        {
            var employee = await HRDataService.GetEmployeeAsync(id);
            if (employee == null)
                return RedirectToAction("Index");

            ViewBag.Employee = employee;

            // Validate
            if (string.IsNullOrWhiteSpace(newPassword))
                ModelState.AddModelError("newPassword", "Nhập mật khẩu");

            if (newPassword != confirmPassword)
                ModelState.AddModelError("confirmPassword", "Mật khẩu không khớp");

            if (!ModelState.IsValid)
                return View();

            // ================= HASH MD5 =================
            string hashedPassword;
            using (MD5 md5 = MD5.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(newPassword);
                var hash = md5.ComputeHash(bytes);
                hashedPassword = BitConverter.ToString(hash).Replace("-", "").ToLower();
            }

            // ================= SAVE =================
            var accountService = new AccountDataService();
            bool result = await accountService.ChangeEmployeePasswordAsync(employee.Email, hashedPassword);

            if (!result)
            {
                ModelState.AddModelError("", "Đổi mật khẩu thất bại");
                return View();
            }

            TempData["Success"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Index");
        }

        // ================= CHANGE ROLE =================
        [HttpGet]
        public async Task<IActionResult> ChangeRole(int id)
        {
            var employee = await HRDataService.GetEmployeeAsync(id);
            if (employee == null)
                return RedirectToAction("Index");

            ViewBag.Employee = employee;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(int id, string[] roles)
        {
            var employee = await HRDataService.GetEmployeeAsync(id);
            if (employee == null)
                return RedirectToAction("Index");

            // Lưu role dạng chuỗi
            employee.RoleNames = string.Join(",", roles ?? new string[] { });

            await HRDataService.UpdateEmployeeAsync(employee);

            TempData["Success"] = "Cập nhật quyền thành công!";
            return RedirectToAction("Index");
        }
    }
}