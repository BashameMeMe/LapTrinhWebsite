using Dapper;
using Microsoft.Data.SqlClient;
using SV22T1020193.DataLayers.Interfaces;
using SV22T1020193.Models.HR;
using SV22T1020193.Models.Partner;
using SV22T1020193.Models.Security;
using System.Data;

namespace SV22T1020193.DataLayers.SQLServer
{
    /// <summary>
    /// Xử lý dữ liệu tài khoản nhân viên (Employee) trên SQL Server
    /// </summary>
    public class EmployeeAccountRepository : IUserAccountRepository
    {
        private readonly string _connectionString;

        public EmployeeAccountRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<UserAccount?> AuthticateAsync(string userName, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Giả định bảng Employees dùng Email làm tên đăng nhập và có 1 trường Password bị ẩn
                // Lưu ý bổ sung trường Password bên SQL Server của bạn nếu có.
                string sql = @"SELECT EmployeeID AS UserId, 
                                      Email AS UserName, 
                                      FullName AS DisplayName, 
                                      Email AS Email, 
                                      Photo, 
                                      RoleNames
                               FROM Employees 
                               WHERE Email = @userName AND Password = @password AND IsWorking = 1";
                return await connection.QueryFirstOrDefaultAsync<UserAccount>(sql, new { userName, password });
            }
        }

        public async Task<bool> ChangePasswordAsync(string userName, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "UPDATE Employees SET Password = @password WHERE Email = @userName";
                int rowsAffected = await connection.ExecuteAsync(sql, new { userName, password });
                return rowsAffected > 0;
            }
        }
        /// <summary>
        /// Thêm mới / Đăng ký tài khoản Nhân viên
        /// </summary>
        public long RegisterEmployee(Employee data, string password)
        {
            long id = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Câu lệnh SQL bám sát các cột trong CSDL: FullName, BirthDate, Address, Phone, Email, Password, Photo, IsWorking, RoleNames
                string sql = @"
                    INSERT INTO Employees (FullName, BirthDate, Address, Phone, Email, Password, Photo, IsWorking)
                    VALUES (@FullName, @BirthDate, @Address, @Phone, @Email, @Password, @Photo, @IsWorking);
                    SELECT SCOPE_IDENTITY();
                ";

                var parameters = new
                {
                    FullName = data.FullName ?? "",
                    BirthDate = data.BirthDate, // Đảm bảo kiểu DateTime hợp lệ
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    Password = password,        // Bơm tham số mật khẩu từ ngoài vào
                    Photo = data.Photo ?? "",
                    IsWorking = data.IsWorking // Thường mặc định là true khi mới tạo
                };

                id = connection.ExecuteScalar<long>(sql, parameters, commandType: CommandType.Text);
            }
            return id;
        }

        // Implement các method rỗng (hoặc ném NotImplementedException) cho các hàm của Customer nếu Interface bắt buộc
        public long RegisterCustomer(Customer data, string password)
        {
            throw new NotImplementedException("Class này chỉ xử lý cho Employee");
        }
    }
}
