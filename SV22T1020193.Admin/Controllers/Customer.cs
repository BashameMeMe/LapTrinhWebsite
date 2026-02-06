using Microsoft.AspNetCore.Mvc;

namespace SV22T1020193.Admin.Controllers
{
    public class Customer : Controller
    {
        public IActionResult Index()
        {
            return View();
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
