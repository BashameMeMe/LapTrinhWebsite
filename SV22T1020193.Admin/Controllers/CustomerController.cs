using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020193.Models.Common;
using SV22T1020193.Models.Partner;

namespace SV22T1020193.Admin.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private const string Customer_search = "CustomerSearchInput";

        // ===================== INDEX =====================
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(Customer_search);
            if (input == null)
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = 5,
                    SearchValue = ""
                };

            return View(input);
        }

        // ===================== SEARCH =====================
        public async Task<IActionResult> Search(PaginationSearchInput input)
        {
            var result = await PartnerDataService.ListCustomersAsync(input);
            ApplicationContext.SetSessionData(Customer_search, input);

            return PartialView("Search", result); // AJAX chuẩn
        }

        // ===================== CREATE =====================
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung khách hàng";

            return View("Edit", new Customer()
            {
                CustomerID = 0,
                IsLocked = false
            });
        }

        // ===================== EDIT =====================
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Cập nhật khách hàng";

            var model = await PartnerDataService.GetCustomerAsync(id);
            if (model == null)
                return RedirectToAction("Index");

            return View("Edit", model);
        }

        // ===================== SAVE =====================
        [HttpPost]
        public async Task<IActionResult> SaveData(Customer data)
        {
            ViewBag.Title = data.CustomerID == 0
                ? "Bổ sung khách hàng"
                : "Cập nhật khách hàng";

            // Validate
            if (string.IsNullOrWhiteSpace(data.CustomerName))
                ModelState.AddModelError(nameof(data.CustomerName), "Nhập tên khách hàng");

            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Nhập email");

            if (string.IsNullOrWhiteSpace(data.Province))
                ModelState.AddModelError(nameof(data.Province), "Chọn tỉnh/thành");

            // Check trùng email
            if (await PartnerDataService.ValidatelCustomerEmailAsync(data.Email, data.CustomerID))
                ModelState.AddModelError(nameof(data.Email), "Email đã tồn tại");

            if (!ModelState.IsValid)
                return View("Edit", data);

            // Chuẩn hóa dữ liệu
            data.ContactName ??= "";
            data.Phone ??= "";
            data.Address ??= "";

            // Lưu DB
            if (data.CustomerID == 0)
                await PartnerDataService.AddCustomerAsync(data);
            else
                await PartnerDataService.UpdateCustomerAsync(data);

            return RedirectToAction("Index");
        }
        // ===================== DELETE (GET) =====================
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await PartnerDataService.GetCustomerAsync(id);
            if (model == null)
                return RedirectToAction("Index");

            ViewBag.AllowDelete = !await PartnerDataService.IsUsedCustomerAsync(id);
            return View(model);
        }

        // ===================== DELETE (POST) =====================
        [HttpPost]
        public async Task<IActionResult> Delete(int id, IFormCollection form)
        {
            bool isUsed = await PartnerDataService.IsUsedCustomerAsync(id);

            if (isUsed)
            {
                TempData["Error"] = "Khách hàng đã có đơn hàng, không thể xóa!";
                return RedirectToAction("Delete", new { id });
            }

            await PartnerDataService.DeleteCustomerAsync(id);
            return RedirectToAction("Index");
        }

        // ===================== CHANGE PASSWORD =====================
        [HttpGet]
        public async Task<IActionResult> ChangePassword(int id)
        {
            var customer = await PartnerDataService.GetCustomerAsync(id);

            if (customer == null)
                return RedirectToAction("Index");

            ViewBag.Customer = customer; // 🔥 QUAN TRỌNG

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(int id, string newPassword, string confirmPassword)
        {
            var customer = await PartnerDataService.GetCustomerAsync(id);

            if (customer == null)
                return RedirectToAction("Index");

            ViewBag.Customer = customer;

            // Validate
            if (string.IsNullOrWhiteSpace(newPassword))
                ModelState.AddModelError("newPassword", "Vui lòng nhập mật khẩu");

            if (newPassword != confirmPassword)
                ModelState.AddModelError("confirmPassword", "Mật khẩu không khớp");

            if (!ModelState.IsValid)
                return View();

            // 🔥 GỌI ĐÚNG HÀM
            await PartnerDataService.ChangeCustomerPasswordAsync(id, newPassword);

            TempData["Success"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Index");
        }
    }
}