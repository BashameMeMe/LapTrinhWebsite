using Dapper;
using Microsoft.Data.SqlClient;
using SV22T1020193.DataLayers.Interfaces;
using SV22T1020193.Models.Partner;
using SV22T1020193.Models.Security;
using System.Data;

namespace SV22T1020193.DataLayers.SQLServer
{
    /// <summary>
    /// Xử lý dữ liệu tài khoản khách hàng (Customer) trên SQL Server
    /// </summary>
    public class CustomerAccountRepository : IUserAccountRepository
    {
        private readonly string _connectionString;

        public CustomerAccountRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<UserAccount?> AuthticateAsync(string userName, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT CustomerID AS UserId, 
                                      Email AS UserName, 
                                      CustomerName AS DisplayName, 
                                      Email AS Email, 
                                      '' AS Photo, 
                                      '' AS RoleNames
                               FROM Customers 
                               WHERE Email = @userName AND Password = @password AND IsLocked = 0";
                return await connection.QueryFirstOrDefaultAsync<UserAccount>(sql, new { userName, password });
            }
        }

        public async Task<bool> ChangePasswordAsync(string userName, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "UPDATE Customers SET Password = @password WHERE Email = @userName";
                int rowsAffected = await connection.ExecuteAsync(sql, new { userName, password });
                return rowsAffected > 0;
            }
        }
        /// <summary>
        /// Thêm khách hàng mới (Đăng ký)
        /// </summary>
        public async Task<int> RegisterCustomerAsync(Customer data, string password)
        {
            int id = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Câu lệnh SQL chèn vào bảng Customers có cột Password
                string sql = @"
                    INSERT INTO Customers (CustomerName, ContactName, Province, Address, Phone, Email, Password, IsLocked)
                    VALUES (@CustomerName, @ContactName, @Province, @Address, @Phone, @Email, @Password, @IsLocked);
                    SELECT CAST(SCOPE_IDENTITY() as int);
                ";

                var parameters = new
                {
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    Password = password, // Mật khẩu truyền từ ngoài vào
                    IsLocked = data.IsLocked
                };

                // Thực thi câu lệnh SQL bất đồng bộ
                id = await connection.ExecuteScalarAsync<int>(sql, parameters, commandType: CommandType.Text);
            }
            return id;
        }
    }
}