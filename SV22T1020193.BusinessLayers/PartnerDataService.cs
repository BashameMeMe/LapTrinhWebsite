using SV22T1020193.DataLayers.Interfaces;
using SV22T1020193.DataLayers.SQLServer;
using SV22T1020193.Models.Common;
using SV22T1020193.Models.Partner;

namespace SV22T1020193.BusinessLayers
{
    /// <summary>
    /// Cung cấp các chức năng xử lý dữ liệu liên quan đến đối tác của hệ thống.
    /// Bao gồm:
    /// Supplier (Nhà cung cấp),
    /// Customer (Khách hàng),
    /// Shipper (Người giao hàng)
    /// </summary>
    internal class PartnerDataService
    {
        private static readonly IGenericRepository<Supplier> supplierDB;
        private static readonly IGenericRepository<Shipper> shipperDB;
        private static readonly ICustomerRepository customerDB;

        /// <summary>
        /// Constructor tĩnh: Khởi tạo các Repository dùng để truy xuất dữ liệu
        /// </summary>
        static PartnerDataService()
        {
            supplierDB = new SupplierRepository(Configuration.ConnectionString);
            shipperDB = new ShipperRepository(Configuration.ConnectionString);
            customerDB = new CustomerRepository(Configuration.ConnectionString);
        }

        #region ===== SUPPLIER =====

        /// <summary>
        /// Lấy danh sách nhà cung cấp theo điều kiện tìm kiếm và phân trang
        /// </summary>
        /// <param name="input">Thông tin tìm kiếm và phân trang</param>
        /// <returns>Danh sách nhà cung cấp</returns>
        public static async Task<PagedResult<Supplier>> ListSupplierAsync(PaginationSearchInput input)
        {
            return await supplierDB.ListAsync(input);
        }

        /// <summary>
        /// Lấy thông tin của một nhà cung cấp theo mã
        /// </summary>
        /// <param name="supplierId">Mã nhà cung cấp</param>
        /// <returns>Thông tin nhà cung cấp</returns>
        public static async Task<Supplier?> GetSupplierAsync(int supplierId)
        {
            return await supplierDB.GetAsync(supplierId);
        }

        /// <summary>
        /// Bổ sung một nhà cung cấp mới
        /// </summary>
        /// <param name="supplier">Thông tin nhà cung cấp cần thêm</param>
        /// <returns>Mã nhà cung cấp được tạo</returns>
        public static async Task<int> AddSupplierAsync(Supplier supplier)
        {
            // TODO: kiểm tra dữ liệu hợp lệ
            return await supplierDB.AddAsync(supplier);
        }

        /// <summary>
        /// Cập nhật thông tin nhà cung cấp
        /// </summary>
        /// <param name="supplier">Thông tin nhà cung cấp cần cập nhật</param>
        /// <returns>True nếu cập nhật thành công</returns>
        public static async Task<bool?> UpdateSupplierAsync(Supplier supplier)
        {
            // TODO: kiểm tra dữ liệu hợp lệ
            return await supplierDB.UpdateAsync(supplier);
        }

        /// <summary>
        /// Xóa một nhà cung cấp
        /// </summary>
        /// <param name="supplierId">Mã nhà cung cấp cần xóa</param>
        /// <returns>
        /// True: xóa thành công  
        /// False: không thể xóa vì đã có dữ liệu liên quan
        /// </returns>
        public static async Task<bool?> DeleteSupplierAsync(int supplierId)
        {
            if (await supplierDB.IsUsedAsync(supplierId))
            {
                return false;
            }
            return await supplierDB.DeleteAsync(supplierId);
        }

        /// <summary>
        /// Kiểm tra nhà cung cấp có đang được sử dụng trong hệ thống hay không
        /// </summary>
        /// <param name="supplierId">Mã nhà cung cấp</param>
        /// <returns>True nếu có dữ liệu liên quan</returns>
        public static async Task<bool?> IsUsedSupplierAsync(int supplierId)
        {
            return await supplierDB.IsUsedAsync(supplierId);
        }

        #endregion

        #region ===== SHIPPER =====

        /// <summary>
        /// Lấy danh sách người giao hàng theo điều kiện tìm kiếm và phân trang
        /// </summary>
        /// <param name="input">Thông tin tìm kiếm và phân trang</param>
        /// <returns>Danh sách người giao hàng</returns>
        public static async Task<PagedResult<Shipper>> ListShipperAsync(PaginationSearchInput input)
        {
            return await shipperDB.ListAsync(input);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một người giao hàng
        /// </summary>
        /// <param name="shipperId">Mã người giao hàng</param>
        /// <returns>Thông tin người giao hàng</returns>
        public static async Task<Shipper?> GetShipperAsync(int shipperId)
        {
            return await shipperDB.GetAsync(shipperId);
        }

        /// <summary>
        /// Bổ sung một người giao hàng mới
        /// </summary>
        /// <param name="shipper">Thông tin người giao hàng</param>
        /// <returns>Mã người giao hàng được tạo</returns>
        public static async Task<int> AddShipperAsync(Shipper shipper)
        {
            // TODO: kiểm tra dữ liệu hợp lệ
            return await shipperDB.AddAsync(shipper);
        }

        /// <summary>
        /// Cập nhật thông tin người giao hàng
        /// </summary>
        /// <param name="shipper">Thông tin người giao hàng</param>
        /// <returns>True nếu cập nhật thành công</returns>
        public static async Task<bool?> UpdateShipperAsync(Shipper shipper)
        {
            return await shipperDB.UpdateAsync(shipper);
        }

        /// <summary>
        /// Xóa một người giao hàng
        /// </summary>
        /// <param name="shipperId">Mã người giao hàng cần xóa</param>
        /// <returns>
        /// True: xóa thành công  
        /// False: không thể xóa vì đã có dữ liệu liên quan
        /// </returns>
        public static async Task<bool?> DeleteShipperAsync(int shipperId)
        {
            if (await shipperDB.IsUsedAsync(shipperId))
            {
                return false;
            }
            return await shipperDB.DeleteAsync(shipperId);
        }

        /// <summary>
        /// Kiểm tra người giao hàng có đang được sử dụng trong hệ thống hay không
        /// </summary>
        /// <param name="shipperId">Mã người giao hàng</param>
        /// <returns>True nếu có dữ liệu liên quan</returns>
        public static async Task<bool?> IsUsedShipperAsync(int shipperId)
        {
            return await shipperDB.IsUsedAsync(shipperId);
        }

        #endregion

        #region ===== CUSTOMER =====

        /// <summary>
        /// Lấy danh sách khách hàng theo điều kiện tìm kiếm và phân trang
        /// </summary>
        /// <param name="input">Thông tin tìm kiếm</param>
        /// <returns>Danh sách khách hàng</returns>
        public static async Task<PagedResult<Customer>> ListCustomerAsync(PaginationSearchInput input)
        {
            return await customerDB.ListAsync(input);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một khách hàng
        /// </summary>
        /// <param name="customerId">Mã khách hàng</param>
        /// <returns>Thông tin khách hàng</returns>
        public static async Task<Customer?> GetCustomerAsync(int customerId)
        {
            return await customerDB.GetAsync(customerId);
        }

        /// <summary>
        /// Thêm mới một khách hàng
        /// </summary>
        /// <param name="customer">Thông tin khách hàng</param>
        /// <returns>Mã khách hàng được tạo</returns>
        public static async Task<int> AddCustomerAsync(Customer customer)
        {
            return await customerDB.AddAsync(customer);
        }

        /// <summary>
        /// Cập nhật thông tin khách hàng
        /// </summary>
        /// <param name="customer">Thông tin khách hàng</param>
        /// <returns>True nếu cập nhật thành công</returns>
        public static async Task<bool?> UpdateCustomerAsync(Customer customer)
        {
            return await customerDB.UpdateAsync(customer);
        }

        /// <summary>
        /// Xóa một khách hàng
        /// </summary>
        /// <param name="customerId">Mã khách hàng cần xóa</param>
        /// <returns>
        /// True: xóa thành công  
        /// False: không thể xóa vì đã có dữ liệu liên quan
        /// </returns>
        public static async Task<bool?> DeleteCustomerAsync(int customerId)
        {
            if (await customerDB.IsUsedAsync(customerId))
            {
                return false;
            }
            return await customerDB.DeleteAsync(customerId);
        }

        /// <summary>
        /// Kiểm tra khách hàng có đang được sử dụng trong hệ thống hay không
        /// </summary>
        /// <param name="customerId">Mã khách hàng</param>
        /// <returns>True nếu có dữ liệu liên quan</returns>
        public static async Task<bool?> IsUsedCustomerAsync(int customerId)
        {
            return await customerDB.IsUsedAsync(customerId);
        }

        #endregion
    }
}