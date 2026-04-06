using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020193.Models.Common;
using SV22T1020193.Models.Partner;

namespace SV22T1020193.Admin.Controllers
{
    [Authorize]
    public class ShipperController : Controller
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

        // ===== GET =====
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung người giao hàng";
            return View("Edit", new Shipper());
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Cập nhật người giao hàng";
            var data = await PartnerDataService.GetShipperAsync(id);
            return View(data);
        }

        // ===== POST =====
        [HttpPost]
        public async Task<IActionResult> Create(Shipper data)
        {
            if (!ModelState.IsValid)
                return View("Edit", data);

            await PartnerDataService.AddShipperAsync(data);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Shipper data)
        {
            if (!ModelState.IsValid)
                return View(data);

            await PartnerDataService.UpdateShipperAsync(data);
            return RedirectToAction("Index");
        }

        // ===== DELETE =====
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