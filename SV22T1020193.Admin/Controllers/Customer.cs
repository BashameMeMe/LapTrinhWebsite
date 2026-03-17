using Microsoft.AspNetCore.Mvc;
using SV22T1020193.Models.Common;

namespace SV22T1020193.Admin.Controllers
{
    public class Customer : Controller
  {/// <summary>
        /// Lưu điều kiện tìm kiếm khách hàng trong session
        /// </summary>
        private const string Customer_search = "CustomerSearchInput";
        /// <summary>
        /// Nhập đầu vào tìm kiếm,Hiển thị kết quả tìm kiếm
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(Customer_search);
            if(input == null)
                input = new PaginationSearchInput()
            {
                Page = 1,
                PageSize = ApplicationContext.PageSize,
                SearchValue = ""
            };
            return View(input);
        }
        /// <summary>
        /// Tìm kiếm và trả về kết quả
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Search(PaginationSearchInput input)
        {
            var result = await PartnerDataService.ListCustomersAsync(input);
            ApplicationContext.SetSessionData(Customer_search, input);
            return View(result);
        }

        // GET: Customer/Create
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung khách hàng";
            return View("Edit");
        }

        // GET: Customer/Edit/5
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Cập nhật khách hàng";
            return View();
        }

        // GET: Customer/Delete/5
        public IActionResult Delete(int id)
        {
            ViewBag.Title = "Xóa khách hàng";
            return View();
        }

        // POST: Customer/Delete/5
        [HttpPost]
        public IActionResult Delete(int id, IFormCollection form)
        {
            // TODO: Thực hiện xóa trong DB
            return RedirectToAction("Index");
        }
        public IActionResult Changepassword(int id)
        {
            return View();
        }
    }
}
