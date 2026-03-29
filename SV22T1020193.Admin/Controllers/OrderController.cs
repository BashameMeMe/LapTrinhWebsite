using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SV22T1020193.Admin.AppCodes;
using SV22T1020193.BusinessLayers;
using SV22T1020193.Models.Catalog;
using SV22T1020193.Models.Sales;
using System.Buffers;

namespace SV22T1020193.Admin.Controllers
{

    public class OrderController : Controller
    {
        private const string PRODUCT_SEARCH = "SearchProductToSale";
        /// <summary>
        /// Nhập đầu vào tìm kiếm và hiển thị kết quả tìm kiếm đơn hàng
        /// </summary>
        /// <returns></returns>

        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Giao diện các chức năng hỗ trợ cho nghiệp vụ tạo đơn hàng mới
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            var input = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH);
            if (input == null) input = new ProductSearchInput()
            {
                Page = 1,
                PageSize = 5,
                SearchValue = ""
            };
            return View();
        }
        public async Task<IActionResult> SearchProduct(ProductSearchInput intput)
        {
            var input = new ProductSearchInput()
            {
                Page = 1,
                PageSize = 5,
                SearchValue = ""
            };
            var result = await CatalogDataService.ListProductsAsync(intput);
            ApplicationContext.SetSessionData(PRODUCT_SEARCH, intput);
            return PartialView(result);
        }
        /// <summary>
        /// Tìm kiếm đơn hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult Search()
        {
            return View();
        }
        /// <summary>
        /// Hiển thị thông tin chi tiết của một đơn hàng
        /// Và đồng thời điều hướng tới các chức năng xử lý trên đơn(id)
        /// </summary>
        /// <param name="id">Mã đơn hàng cần cập nhật</param>
        /// <returns></returns>
        public IActionResult Detail(int id)
        {
            return View();
        }
        /// <summary>
        /// hiển thị giỏ hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult ShowCart()
        {
            var cart = ShoppingCartService.GetShoppingCart();
            return PartialView(cart);
        }
        [HttpPost]
        public async Task<IActionResult> AddCartItem(int productId, int quantity, decimal price)
        {
            if (quantity <= 0)
            {
                return Json(new ApiResult(0, "Số lượng không hợp lệ"));
            }
            if (price < 0) {
                return Json(new ApiResult(0, "Mặt hàng không tồn tại"));
            }
            var product = await CatalogDataService.GetProductAsync(productId);
            if (product == null)
            {
                return Json(new ApiResult(0, "Mặt hàng không tồn tại"));
            }
            if (!product.IsSelling)
            {
                return Json(new ApiResult(0, "Mặt hàng này đã ngừng bán"));
            }
            ShoppingCartService.AddCartItem(new SV22T1020193.Models.Sales.OrderDetailViewInfo()
            {
                ProductID = productId,
                Quantity = quantity,
                SalePrice = price,
                ProductName = product.ProductName,
                Unit = product.Unit,
                Photo = product.Photo ?? "nophoto.png"
            });
            return Json(new ApiResult(1));
        }
        /// <summary>
        /// Cập nhật thông tin (số lượng, giá bán) của một mặt hàng
        /// Trong giỏ hàng,Trong một đơn hàng
        /// </summary>
        /// <param name="id">0:Giỏ hàng,Khác 0: mã đơn hàng cần xử lý</param>
        /// <param name="productId">Mã mặt hàng cần cập nhật</param>
        /// <returns></returns>
        public IActionResult EditCartItem(int productId = 0)
        {
            var item = ShoppingCartService.GetCartItem(productId);
            return PartialView(item);
        }
        public async Task<IActionResult> UpdateCartItemAsync(int productId,int quantity,decimal salePrice)
        {
            if (productId <= 0)
            {
                return Json(new ApiResult(0, "Mặt hàng không hợp lệ"));
            }

            // 2. Kiểm tra số lượng
            if (quantity <= 0)
            {
                return Json(new ApiResult(0, "Số lượng phải lớn hơn 0"));
            }

            // 3. Kiểm tra giá
            if (salePrice < 0)
            {
                return Json(new ApiResult(0, "Giá bán không hợp lệ"));
            }

            // 4. Kiểm tra sản phẩm có tồn tại không
            var product = await CatalogDataService.GetProductAsync(productId);
            if (product == null)
            {
                return Json(new ApiResult(0, "Mặt hàng không tồn tại"));
            }

            // 5. Kiểm tra còn bán không
            if (!product.IsSelling)
            {
                return Json(new ApiResult(0, "Mặt hàng đã ngừng bán"));
            }

            // 6. (QUAN TRỌNG) Không cho sửa giá thấp hơn giá gốc (tránh hack)
            if (salePrice < product.Price)
            {
                return Json(new ApiResult(0, "Giá bán không được nhỏ hơn giá niêm yết"));
            }

            // 7. (Tuỳ chọn) Giới hạn số lượng
            if (quantity > 1000)
            {
                return Json(new ApiResult(0, "Số lượng quá lớn"));
            }

            // 8. Cập nhật giỏ hàng
            ShoppingCartService.UpdateCartItem(productId, quantity, salePrice);

            return Json(new ApiResult(1, "Cập nhật thành công"));
        }
        public IActionResult DeleteCartItem(int productId = 0)
        {
            if(Request.Method == "POST"){
                ShoppingCartService.RemoveCartItem(productId);
                return Json(new ApiResult(1));
            }
            var item = ShoppingCartService.GetCartItem(productId);
            return PartialView(item);
        }
        public IActionResult ClearCart()
        {
            if (Request.Method == "POST")
            {
                ShoppingCartService.ClearCart();
                return Json(new ApiResult(1));
            }
            return PartialView();
        }
        public async Task<IActionResult> CreateOrder(int customerId, string province, string address)
        {
            // 1. Kiểm tra khách hàng
            if (customerId <= 0)
            {
                return Json(new ApiResult(0, "Vui lòng chọn khách hàng"));
            }

            // 2. Kiểm tra địa chỉ giao hàng
            if (string.IsNullOrWhiteSpace(province))
            {
                return Json(new ApiResult(0, "Vui lòng chọn tỉnh/thành"));
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                return Json(new ApiResult(0, "Vui lòng nhập địa chỉ giao hàng"));
            }

            // 3. Lấy giỏ hàng
            var cart = ShoppingCartService.GetShoppingCart();

            if (cart == null || !cart.Any())
            {
                return Json(new ApiResult(0, "Giỏ hàng đang trống"));
            }

            // 4. Kiểm tra từng mặt hàng trong giỏ
            foreach (var item in cart)
            {
                // Số lượng
                if (item.Quantity <= 0)
                {
                    return Json(new ApiResult(0, $"Số lượng không hợp lệ: {item.ProductName}"));
                }

                // Giá
                if (item.SalePrice < 0)
                {
                    return Json(new ApiResult(0, $"Giá không hợp lệ: {item.ProductName}"));
                }

                // Kiểm tra sản phẩm còn tồn tại & đang bán
                var product = await CatalogDataService.GetProductAsync(item.ProductID);
                if (product == null)
                {
                    return Json(new ApiResult(0, $"Mặt hàng không tồn tại: {item.ProductName}"));
                }

                if (!product.IsSelling)
                {
                    return Json(new ApiResult(0, $"Mặt hàng đã ngừng bán: {item.ProductName}"));
                }
            }

            // 5. Tạo đơn hàng
            var order = new Order()
            {
                CustomerID = customerId,
                DeliveryProvince = province,
                DeliveryAddress = address,
                OrderTime = DateTime.Now,
                Status = 0 // 0 = đơn mới
            };

            int orderID = await SalesDataService.AddOrderAsync(order);

            // 6. Lưu chi tiết đơn hàng
            foreach (var item in cart)
            {
                item.OrderID = orderID;

                // (Khuyến nghị) lấy lại giá từ DB để tránh sửa giá
                var product = await CatalogDataService.GetProductAsync(item.ProductID);
                item.SalePrice = product.Price;

                await SalesDataService.AddDetailAsync(item);
            }

            // 7. Xóa giỏ hàng
            ShoppingCartService.ClearCart();

            return Json(new ApiResult(1, "Tạo đơn hàng thành công", orderID));
        }
        public IActionResult Accept(int id = 0)
        {
            return View();
        }
        public IActionResult Shipping(int id)
        {
            return View();
        }
        public IActionResult Finish(int id)
        {
            return View();
        }
        public IActionResult Reject(int id)
        {
            return View();
        }
        public IActionResult Cancel(int id)
        {
            return View();
        }
        public IActionResult Delete(int id)
        {
            return View();
        }
    }
}
