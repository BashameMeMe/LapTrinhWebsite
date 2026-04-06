using Microsoft.AspNetCore.Http;
using SV22T1020193.Models.Sales;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json; // Thêm thư viện này

namespace SV22T1020193.Shop.AppCodes
{
    public static class ShoppingCartService
    {
        private static readonly string SESSION_KEY = "ShoppingCart";

        // Lấy giỏ hàng từ session (truyền HttpContext vào)
        public static List<OrderDetailViewInfo> GetShoppingCart(HttpContext context)
        {
            var sessionData = context.Session.GetString(SESSION_KEY);
            List<OrderDetailViewInfo> cart = null;

            if (!string.IsNullOrEmpty(sessionData))
            {
                cart = JsonSerializer.Deserialize<List<OrderDetailViewInfo>>(sessionData);
            }

            if (cart == null)
            {
                cart = new List<OrderDetailViewInfo>();
            }
            return cart;
        }

        // Lấy thông tin 1 sản phẩm
        public static OrderDetailViewInfo? GetCartItem(HttpContext context, int productId)
        {
            var cart = GetShoppingCart(context);
            return cart.FirstOrDefault(c => c.ProductID == productId);
        }

        // Thêm sản phẩm
        public static void AddToCart(HttpContext context, OrderDetailViewInfo item)
        {
            var cart = GetShoppingCart(context);
            var existingItem = cart.FirstOrDefault(c => c.ProductID == item.ProductID);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
                existingItem.SalePrice = item.SalePrice;
            }
            else
            {
                cart.Add(item);
            }

            // LƯU LẠI VÀO SESSION CHUẨN ASP.NET CORE
            context.Session.SetString(SESSION_KEY, JsonSerializer.Serialize(cart));
        }

        // Cập nhật
        public static void UpdateCartItem(HttpContext context, int productId, int quantity, decimal salePrice)
        {
            var cart = GetShoppingCart(context);
            var existingItem = cart.FirstOrDefault(c => c.ProductID == productId);
            if (existingItem != null)
            {
                existingItem.Quantity = quantity;
                existingItem.SalePrice = salePrice;
                context.Session.SetString(SESSION_KEY, JsonSerializer.Serialize(cart));
            }
        }

        // Xóa sản phẩm
        public static void RemoveFromCart(HttpContext context, int productId)
        {
            var cart = GetShoppingCart(context);
            var existingItem = cart.FirstOrDefault(c => c.ProductID == productId);
            if (existingItem != null)
            {
                cart.Remove(existingItem);
                context.Session.SetString(SESSION_KEY, JsonSerializer.Serialize(cart));
            }
        }

        // Xóa toàn bộ giỏ hàng
        public static void ClearCart(HttpContext context)
        {
            context.Session.Remove(SESSION_KEY);
        }
    }
}