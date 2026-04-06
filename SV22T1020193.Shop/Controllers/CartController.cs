using Microsoft.AspNetCore.Mvc;
using SV22T1020193.Shop.AppCodes;
using SV22T1020193.Models.Sales;

namespace SV22T1020193.Shop.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            // Thêm tham số HttpContext
            var cart = ShoppingCartService.GetShoppingCart(HttpContext);
            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int ProductID, string ProductName, decimal SalePrice, int Quantity = 1)
        {
            var item = new SV22T1020193.Models.Sales.OrderDetailViewInfo()
            {
                ProductID = ProductID,
                ProductName = ProductName,
                SalePrice = SalePrice,
                Quantity = Quantity
            };

            // Thêm tham số HttpContext
            ShoppingCartService.AddToCart(HttpContext, item);

            return Json(new { success = true });
        }

        public IActionResult Remove(int id)
        {
            ShoppingCartService.RemoveFromCart(HttpContext, id);
            return RedirectToAction("Index");
        }

        public IActionResult Clear()
        {
            ShoppingCartService.ClearCart(HttpContext);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var item = ShoppingCartService.GetCartItem(HttpContext, id);
            if (item == null)
            {
                return NotFound();
            }
            return PartialView("_EditCartItem", item);
        }

        [HttpPost]
        public IActionResult Update(int productId, int quantity, decimal salePrice)
        {
            if (quantity <= 0)
            {
                ShoppingCartService.RemoveFromCart(HttpContext, productId);
            }
            else
            {
                ShoppingCartService.UpdateCartItem(HttpContext, productId, quantity, salePrice);
            }

            return Json(new { success = true });
        }

        public IActionResult Checkout()
        {
            var cart = ShoppingCartService.GetShoppingCart(HttpContext);
            if (cart == null || cart.Count == 0)
            {
                TempData["Message"] = "Giỏ hàng rỗng, không thể xác nhận mua hàng!";
                return RedirectToAction("Index");
            }

            // ... Logic đặt hàng của bạn ...

            ShoppingCartService.ClearCart(HttpContext);
            TempData["Message"] = "Đặt hàng thành công!";
            return RedirectToAction("Index", "Home");
        }
    }
}