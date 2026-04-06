using Microsoft.AspNetCore.Mvc;
using SV22T1020193.BusinessLayers;
using SV22T1020193.Models.Catalog;
using SV22T1020193.Models.Common;

namespace SV22T1020193.Shop.Controllers
{
    public class HomeController : Controller
    {
        private const int PAGE_SIZE = 8;

        public async Task<IActionResult> Index(ProductSearchInput input)
        {
            var topProducts = await CatalogDataService.GetTopProductsAsync(8);
            return View(topProducts);

        }

        public IActionResult Privacy() => View();
    }
}