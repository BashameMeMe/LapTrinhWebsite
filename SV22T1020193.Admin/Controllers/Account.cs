using Microsoft.AspNetCore.Mvc;

namespace SV22T1020193.Admin.Controllers
{
    public class Account : Controller
    {
        public IActionResult login()
        {
            return View();
        }
        public IActionResult logout() 
        { 
            return RedirectToAction("Login");
        }
        public IActionResult changepassword()
        {
            return View();
        }
    }
}
