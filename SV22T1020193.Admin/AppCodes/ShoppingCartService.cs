using SV22T1020193.Models.Sales;

namespace SV22T1020193.Admin.AppCodes
{
    public class ShoppingCartService
    {
        /// <summary>
        /// Biến lưu trong session
        /// </summary>
        /// 
        private const String CART = "ShoppingCart";
        //Lấy giỏ hàng từ session
        public static List<OrderDetailViewInfo> GetShoppingCart()
        {
            var cart = ApplicationContext.GetSessionData<List<OrderDetailViewInfo>>(CART);
            if (cart == null)
            {
                cart = new List<OrderDetailViewInfo>();
                ApplicationContext.SetSessionData(CART, cart);
            }
            return cart;
        }
        //Lấy một thông tin khách hàng
        public static OrderDetailViewInfo? GetCartItem(int productID)
        {
            var cart = GetShoppingCart();
            return cart.Find(m => m.ProductID == productID);
        }
        //Theêm hàng vào giỏ hàng
        public static void AddCartItem(OrderDetailViewInfo item)
        {
            var cart = GetShoppingCart();
            var existItem = cart.Find(m =>m.ProductID == item.ProductID);
            if (existItem == null) { 
                cart.Add(item);
            }
            else
            {
                existItem.Quantity += item.Quantity;
                existItem.SalePrice = item.SalePrice;
            }
            ApplicationContext.SetSessionData(CART, cart);
        }
        //Cập nhật số lượng giá và hàng trong giỏ hàng
        public static void UpdateCartItem(int productId,int  quantity,int salePrice)
        {
            var cart = GetShoppingCart();
            var item = cart.Find(m => m.ProductID == productId);
            if (item == null)
            {
                item.Quantity = quantity;
                item.SalePrice = salePrice;
                ApplicationContext.SetSessionData(CART, cart);
            }
        }
        //Xóa một mặt hàng ra giỏi mặt hàng
        public static void RemoveCartItem(int productID)
        {
            var cart = GetShoppingCart();
            int index = cart.FindIndex(m => m.ProductID == productID);
            if(index >= 0)
            {
                cart.RemoveAt(index);
                ApplicationContext.SetSessionData(CART, cart);
            }
        }
        //Xóa giỏ hàng
        public static void ClearCart()
        {
            var cart = new List<OrderDetailViewInfo>();
            ApplicationContext.SetSessionData(CART, cart);
        }
    }
}
