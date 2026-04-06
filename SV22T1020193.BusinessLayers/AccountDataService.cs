using SV22T1020193.DataLayers.Interfaces;
using SV22T1020193.DataLayers.SQLServer;
using SV22T1020193.Models.Security;

namespace SV22T1020193.BusinessLayers
{
    /// <summary>
    /// Cung cấp các chức năng nghiệp vụ liên quan đến dữ liệu tài khoản
    /// </summary>
    public class AccountDataService
    {
        private readonly IUserAccountRepository _employeeAccountDB;
        private readonly IUserAccountRepository _customerAccountDB;

        /// <summary>
        /// Khởi tạo AccountDataService. 
        /// Connection string thường được tiêm (inject) từ appsettings.json thông qua Program.cs
        /// </summary>
        /// <param name="connectionString">Chuỗi kết nối CSDL</param>
        public AccountDataService(string connectionString)
        {
            // Khởi tạo các repository xử lý dữ liệu tương ứng
            _employeeAccountDB = new EmployeeAccountRepository(connectionString);
            _customerAccountDB = new CustomerAccountRepository(connectionString);
        }

        #region Xử lý tài khoản Nhân viên (Dành cho Admin)

        /// <summary>
        /// Kiểm tra thông tin đăng nhập của nhân viên
        /// </summary>
        public async Task<UserAccount?> AuthorizeEmployeeAsync(string userName, string password)
        {
            // Lưu ý: interface của bạn đang viết là AuthticateAsync (thiếu chữ 'en' ở giữa), 
            // tôi giữ nguyên để khớp với IUserAccountRepository.cs của bạn.
            return await _employeeAccountDB.AuthticateAsync(userName, password);
        }

        /// <summary>
        /// Thay đổi mật khẩu của nhân viên
        /// </summary>
        public async Task<bool> ChangeEmployeePasswordAsync(string userName, string password)
        {
            return await _employeeAccountDB.ChangePasswordAsync(userName, password);
        }

        #endregion

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

        #endregion
    }
}