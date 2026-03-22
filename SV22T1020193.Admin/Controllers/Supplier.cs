using Microsoft.AspNetCore.Mvc;
using SV22T1020193.Models.Common;

namespace SV22T1020193.Admin.Controllers
{
    public class Supplier : Controller
    {
        /// <summary>
        /// Lưu điều kiện tìm kiếm nhà cung cấp trong session
        /// </summary>
        private const string Supplier_search = "SupplierSearchInput";

        /// <summary>
        /// Nhập đầu vào tìm kiếm, hiển thị kết quả tìm kiếm
        /// </summary>
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(Supplier_search);
            if (input == null)
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = 5,
                    SearchValue = ""
                };

            return View(input);
        }

        /// <summary>
        /// Tìm kiếm và trả về kết quả
        /// </summary>
        public async Task<IActionResult> Search(PaginationSearchInput input)
        {
            var result = await PartnerDataService.ListSuppliersAsync(input);
            ApplicationContext.SetSessionData(Supplier_search, input);
            return View(result);
        }

        /// <summary>
        /// Bổ sung nhà cung cấp
        /// </summary>
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhà cung cấp";
            return View("Edit");
        }

        /// <summary>
        /// Cập nhật nhà cung cấp
        /// </summary>
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Cập nhật nhà cung cấp";

            var data = await PartnerDataService.GetSupplierAsync(id);
            return View(data);
        }

        /// <summary>
        /// Xóa nhà cung cấp (GET)
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            ViewBag.Title = "Xóa nhà cung cấp";

            var data = await PartnerDataService.GetSupplierAsync(id);
            return View(data);
        }

        /// <summary>
        /// Xóa nhà cung cấp (POST)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Delete(int id, IFormCollection form)
        {
            await PartnerDataService.DeleteSupplierAsync(id);
            return RedirectToAction("Index");
        }
    }
}