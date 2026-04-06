using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020193.BusinessLayers;
using SV22T1020193.Models.Common;
using SV22T1020193.Models.Partner;

namespace SV22T1020193.Admin.Controllers
{
    [Authorize]
    public class SupplierController : Controller
    {
        /// <summary>
        /// Lưu điều kiện tìm kiếm trong session
        /// </summary>
        private const string Supplier_search = "SupplierSearchInput";

        /// <summary>
        /// Hiển thị trang chính
        /// </summary>
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(Supplier_search);

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

        /// <summary>
        /// Tìm kiếm (AJAX)
        /// </summary>
        public async Task<IActionResult> Search(PaginationSearchInput input)
        {
            var result = await PartnerDataService.ListSuppliersAsync(input);

            // lưu lại điều kiện tìm kiếm
            ApplicationContext.SetSessionData(Supplier_search, input);

            return PartialView("Search", result);
        }

        /// <summary>
        /// Thêm mới
        /// </summary>
        /// 
        public async Task<IActionResult> Create()
        {
            ViewBag.Title = "Bổ sung nhà cung cấp";
            ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();

            return View("Edit", new Supplier());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Supplier data)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();
                return View("Edit", data);
            }

            await PartnerDataService.AddSupplierAsync(data);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Cập nhật nhà cung cấp";

            var data = await PartnerDataService.GetSupplierAsync(id);
            ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();

            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Supplier data)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();
                return View(data);
            }

            await PartnerDataService.UpdateSupplierAsync(data);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Xóa (GET)
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            ViewBag.Title = "Xóa nhà cung cấp";

            var data = await PartnerDataService.GetSupplierAsync(id);
            return View(data);
        }

        /// <summary>
        /// Xóa (POST)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Delete(int id, IFormCollection form)
        {
            await PartnerDataService.DeleteSupplierAsync(id);
            return RedirectToAction("Index");
        }
    }
}