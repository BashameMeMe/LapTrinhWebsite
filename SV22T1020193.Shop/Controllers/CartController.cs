using Microsoft.AspNetCore.Mvc;
using SV22T1020193.BusinessLayers;
using SV22T1020193.Models.Sales;
using SV22T1020193.Shop.AppCodes;

namespace SV22T1020193.Shop.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            var cart = ShoppingCartService.GetShoppingCart(HttpContext);
            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int ProductID, string ProductName, decimal SalePrice, int Quantity = 1)
        {
            var item = new OrderDetailViewInfo()
            {
                ProductID = ProductID,
                ProductName = ProductName,
                SalePrice = SalePrice,
                Quantity = Quantity
            };
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
            if (item == null) return NotFound();
            return PartialView("_EditCartItem", item);
        }

        [HttpPost]
        public IActionResult Update(int productId, int quantity, decimal salePrice)
        {
            if (quantity <= 0)
                ShoppingCartService.RemoveFromCart(HttpContext, productId);
            else
                ShoppingCartService.UpdateCartItem(HttpContext, productId, quantity, salePrice);

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var cart = ShoppingCartService.GetShoppingCart(HttpContext);
            if (cart.Count == 0)
                return RedirectToAction("Index");

            // Lấy danh sách tỉnh thành từ DictionaryDataService
            ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();

            return View(cart);
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> InitOrder(string deliveryProvince, string deliveryAddress)
        {
            // 1. Lấy giỏ hàng từ ShoppingCartService
            var cart = ShoppingCartService.GetShoppingCart(HttpContext);
            if (cart.Count == 0)
            {
                TempData["Message"] = "Giỏ hàng của bạn đang trống, không thể đặt hàng.";
                return RedirectToAction("Index");
            }

            // 2. Kiểm tra thông tin đầu vào
            if (string.IsNullOrEmpty(deliveryProvince) || string.IsNullOrEmpty(deliveryAddress))
            {
                TempData["Message"] = "Vui lòng chọn Tỉnh/Thành và nhập địa chỉ giao hàng.";
                return RedirectToAction("Checkout");
            }

            // 3. Lấy CustomerID từ User đang đăng nhập (Tránh lỗi FK_Orders_Customers)
            // Lưu ý: User.Identity.Name thường trả về UserName, ta cần lấy UserId từ Claim
            var userClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userClaim == null)
            {
                // Nếu chưa đăng nhập hoặc không lấy được ID, điều hướng về trang Login
                return RedirectToAction("Login", "Account");
            }
            int customerId = int.Parse(userClaim.Value);

            try
            {
                // 4. Tạo đối tượng Order (Phần Master)
                var order = new Order()
                {
                    CustomerID = customerId,
                    OrderTime = DateTime.Now,
                    DeliveryProvince = deliveryProvince,
                    DeliveryAddress = deliveryAddress,
                    Status = OrderStatusEnum.New // Mặc định là đơn hàng mới
                };

                // 5. Lưu đơn hàng vào Database và nhận lại OrderID vừa sinh ra
                int orderID = await SalesDataService.AddOrderAsync(order);

                if (orderID > 0)
                {
                    // 6. Lưu các mặt hàng trong giỏ vào bảng OrderDetail (Phần Details)
                    foreach (var item in cart)
                    {
                        await SalesDataService.AddDetailAsync(new OrderDetail()
                        {
                            OrderID = orderID,
                            ProductID = item.ProductID,
                            Quantity = item.Quantity,
                            SalePrice = item.SalePrice
                        });
                    }

                    // 7. Xóa giỏ hàng sau khi đặt hàng thành công
                    ShoppingCartService.ClearCart(HttpContext);

                    // Chuyển đến trang hoàn tất hoặc trang chi tiết đơn hàng
                    TempData["SuccessMessage"] = "Chúc mừng! Đơn hàng của bạn đã được đặt thành công.";
                    return RedirectToAction("Finish", new { id = orderID });
                }
                else
                {
                    ModelState.AddModelError("", "Không thể tạo đơn hàng. Vui lòng thử lại.");
                    return RedirectToAction("Checkout");
                }
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần thiết
                TempData["Message"] = "Có lỗi xảy ra trong quá trình xử lý: " + ex.Message;
                return RedirectToAction("Checkout");
            }
        }
        public IActionResult Finish(int id)
        {
            ViewBag.OrderID = id;
            TempData["Message"] = "Đặt hàng thành công!";
            return View();
        }
    }
}