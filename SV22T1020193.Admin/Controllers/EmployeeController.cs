using Microsoft.AspNetCore.Mvc;

namespace SV22T1020193.Admin.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Edit(int id)
        {
            return View();
        }
        public IActionResult Delete(int id)
        {
            return View();
        }
        public IActionResult Changepassword(int id)
        {
            return View();
        }
        public IActionResult Changerole(int id)
        {
            return View();
        }

    }
}
