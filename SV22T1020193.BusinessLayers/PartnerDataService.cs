using SV22T1020193.DataLayers.Interfaces;
using SV22T1020193.DataLayers.SQLServer;
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

    }
}
