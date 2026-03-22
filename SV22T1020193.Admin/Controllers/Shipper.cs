using Microsoft.AspNetCore.Mvc;
using SV22T1020193.Models.Common;

namespace SV22T1020193.Admin.Controllers
{
    public class Shipper : Controller
    {
        private const string Shipper_search = "ShipperSearchInput";

        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(Shipper_search);
            if (input == null)
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = 5,
                    SearchValue = ""
                };

            return View(input);
        }

        public async Task<IActionResult> Search(PaginationSearchInput input)
        {
            var result = await PartnerDataService.ListShippersAsync(input);
            ApplicationContext.SetSessionData(Shipper_search, input);

            return PartialView("Search", result); // AJAX
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung người giao hàng";
            return View("Edit");
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Cập nhật người giao hàng";
            var data = await PartnerDataService.GetShipperAsync(id);
            return View(data);
        }

        public async Task<IActionResult> Delete(int id)
        {
            ViewBag.Title = "Xóa người giao hàng";
            var data = await PartnerDataService.GetShipperAsync(id);
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, IFormCollection form)
        {
            await PartnerDataService.DeleteShipperAsync(id);
            return RedirectToAction("Index");
        }
    }
}