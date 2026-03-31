using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020193.Models.Common;
using SV22T1020193.Models.Partner;
namespace SV22T1020193.Admin.Controllers
{
    [Authorize]
    public class CustomerController : Controller
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
                PageSize = 5,
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
            var model = new Customer()
            {
                CustomerID = 0,
                IsLocked = false
            };
            return View("Edit",model);
        }

        // GET: Customer/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Cập nhật khách hàng";
            var model = await PartnerDataService.GetCustomerAsync(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveDaTa(Customer data)
        {
            ViewBag.Title = data.CustomerID == 0 ? "Bổ sung khách hàng" : "cập nhật thông tin khách hàng";
            //Kiểm tra dữ liệu đầu vào có hợp lệ không
            //Sử dụng ModelState để lưu trữ các tình huống(thông báo) lỗi và gửi thông báo lỗi cho View
            //Giải thiết: chỉ cần nhập tên,email và tỉnh thành
         
            if (string.IsNullOrWhiteSpace(data.CustomerName))
                ModelState.AddModelError(nameof(data.CustomerName),"Nhập Tên đi thằng Ngu");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Nhập Email thằng Ngu");
            else if(!await PartnerDataService.ValidatelCustomerEmailAsync(data.Email,data.CustomerID))
                ModelState.AddModelError(nameof(data.Email), "Không thể sử dụng email này");
            if (string.IsNullOrWhiteSpace(data.Province))
                ModelState.AddModelError(nameof(data.Province), "Nhập tỉnh/thành đi thằng Ngu");
            if (!ModelState.IsValid)
            {
                return View("Edit",data);
            }
            //(Tùy chọn) Hiệu chỉnh dữ liệu theo qui tắc của phần mềm
            if (string.IsNullOrWhiteSpace(data.ContactName)) data.ContactName = data.ContactName;
            if (string.IsNullOrEmpty(data.Phone)) data.Phone = data.Phone ="";
            if (string.IsNullOrEmpty(data.Address)) data.Address = data.Address = "";
            // Lưu vào CSDL
            if (data.CustomerID == 0)
            {
                await PartnerDataService.AddCustomerAsync(data);
            }
            else
            {
                await PartnerDataService.UpdateCustomerAsync(data);
            }
            return RedirectToAction("Index");
        }

        // GET: Customer/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if(Request.Method =="POST")
            {
                await PartnerDataService.DeleteSupplierAsync(id);
                return RedirectToAction("Index");
            }
            //GET
            var model = await PartnerDataService.GetCustomerAsync(id);
            if (model == null) 
                return RedirectToAction("Index");
             ViewBag.CanDelete = !await PartnerDataService.IsUsedCustomerAsync(id);
            return View(model);
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
