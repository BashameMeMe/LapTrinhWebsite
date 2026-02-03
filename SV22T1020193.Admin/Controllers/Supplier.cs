using Microsoft.AspNetCore.Mvc;

namespace SV22T1020193.Admin.Controllers
{
    public class Supplier : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhà cung cấp";
            return View("Edit");
        }
        public IActionResult Edit()
        {
            ViewBag.Title = "Cập nhật nhà cung cấp";
            return View();
        }
        public IActionResult Delete()
        {
            return View();
        }
    }
}
