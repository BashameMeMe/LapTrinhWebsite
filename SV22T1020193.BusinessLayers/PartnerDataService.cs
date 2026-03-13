using SV22T1020193.DataLayers.Interfaces;
using SV22T1020193.DataLayers.SQLServer;
using SV22T1020193.Models.Common;
using SV22T1020193.Models.Partner;

namespace SV22T1020193.BusinessLayers
{
    /// <summary>
    /// Cung cấp các tính năng xử lý dữ liệu liên quan đến doói tác của hệ thống.
    /// Bao gồm: Supplier ( nhà cung cấp), Customer(Khách hàng),Shipper(người giao hàng)
    /// </summary>
    internal class PartnerDataService
    {
        private static readonly IGenericRepository<Supplier> supplierDB;
        private static readonly IGenericRepository<Shipper> shipperDB;
        private static readonly ICustomerRepository customerdb;
        /// <summary>
        /// Constructor(Hàm Tạo)
        /// </summary>
        static PartnerDataService()
        {
            supplierDB = new SupplierRepository(Configuration.ConnectionString);
            shipperDB = new ShipperRepository(Configuration.ConnectionString);
            customerdb = new CustomerRepository(Configuration.ConnectionString);
        }
        //==Các chức năng liên quan đến Nhà cung cấp
        public static async Task<PagedResult<Supplier>> ListSupplierAsync(PaginationSearchInput Input)
        {
            return await supplierDB.ListAsync(Input);
        }
        /// <summary>
        /// Lấy thông tin của một nhà cung cấp có mã là <param name="supplierId"></param>
        /// </summary>
        /// <param name="supplierId"></param>
        /// <returns></returns>
        public static async Task<Supplier?> GetSupplierAsync(int supplierId)
        {
            return await supplierDB.GetAsync(supplierId);
        }
        /// <summary>
        /// Bổ sung một nhà cung cấp mới
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns>Mã của nhà cung cấp được bổ sung</returns>
        public static async Task<int> AddSupplierAsync(Supplier supplier)
        {
            //TODO: Kiểm tra tính hợp lệ của dữ liệu trước khi bổ sung
            return await supplierDB.AddAsync(supplier);
        }
        /// <summary>
        /// Cập nhật nhà cung cấp
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        public static async Task<bool?> UpdateSupplierAsync(Supplier supplier)
        {
            //TODO: Kiểm tra tính hợp lệ của dữ liệu trước khi bổ sung
            return await supplierDB.UpdateAsync(supplier);
        }
        /// <summary>
        /// Xóa nhà cung cấp
        /// </summary>
        /// <param name="supplierId"></param>
        /// <returns></returns>
        public static async Task<bool?> DeleteSupplierAsync(int supplierId)
        {
            //TODO: Kiểm tra tính hợp lệ của dữ liệu trước khi bổ sung
            if(await supplierDB.IsUsedAsync(supplierId))
            {
                return false;
            }
            return await supplierDB.DeleteAsync(supplierId);
        }
        /// <summary>
        /// Kiểm tra xem một nhà cung cấp có mặt hàng liên quan hay không(để kt xóa được k)
        /// </summary>
        /// <param name="supplierId"></param>
        /// <returns></returns>
        public static async Task<bool?> IsUsedSupplierAsync(int supplierId)
        {
            //TODO: Kiểm tra tính hợp lệ của dữ liệu trước khi bổ sung
            return await supplierDB.IsUsedAsync(supplierId);
        }


        //==Các chức năng liên quan đến Người giao hàng
    }
}
