using SV22T1020193.DataLayers.Interfaces;
using SV22T1020193.DataLayers.SQLServer;
using SV22T1020193.Models.Partner;
using SV22T1020193.Models.Security;
using System.Threading.Tasks;

namespace SV22T1020193.BusinessLayers
{
    public class AccountDataService
    {
        private readonly IUserAccountRepository _employeeAccountDB;
        private readonly IUserAccountRepository _customerAccountDB;

        /// <summary>
        /// Khởi tạo AccountDataService sử dụng chuỗi kết nối từ lớp Configuration
        /// </summary>
        public AccountDataService()
        {
            // Lấy chuỗi kết nối trực tiếp từ Configuration
            string connectionString = Configuration.ConnectionString;

            _employeeAccountDB = new EmployeeAccountRepository(connectionString);
            _customerAccountDB = new CustomerAccountRepository(connectionString);
        }

        // ... Các phương thức cũ giữ nguyên ...

        #region Xử lý tài khoản Khách hàng (Dành cho Shop)

        /// <summary>
        /// Kiểm tra thông tin đăng nhập của khách hàng
        /// </summary>
        public async Task<UserAccount?> AuthorizeCustomerAsync(string userName, string password)
        {
            return await _customerAccountDB.AuthticateAsync(userName, password);
        }

        /// <summary>
        /// Thay đổi mật khẩu của khách hàng
        /// </summary>
        public async Task<bool> ChangeCustomerPasswordAsync(string userName, string password)
        {
            return await _customerAccountDB.ChangePasswordAsync(userName, password);
        }

        /// <summary>
        /// Đăng ký tài khoản khách hàng mới
        /// </summary>
        public async Task<int> RegisterCustomerAsync(Customer data, string password)
        {
            // Ép kiểu để gọi hàm RegisterCustomerAsync chỉ có trong CustomerAccountRepository
            var customerRepo = _customerAccountDB as CustomerAccountRepository;
            if (customerRepo != null)
            {
                return await customerRepo.RegisterCustomerAsync(data, password);
            }
            return 0; // Thất bại
        }

        #endregion
    }
}