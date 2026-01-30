using Microsoft.AspNetCore.Mvc;

namespace SV22T1020193.Admin.Controllers
{
    public class Order : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
    }
}
