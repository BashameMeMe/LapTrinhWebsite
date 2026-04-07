using Dapper;
using Microsoft.Data.SqlClient;
using SV22T1020193.Models.Partner;
using SV22T1020193.DataLayers.Interfaces;
using SV22T1020193.Models.Common;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace SV22T1020193.DataLayers.SQLServer
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly string _connectionString;

        public CustomerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // ================== MD5 HASH ==================
        private string GetMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }

        // ================== ADD ==================
        public async Task<int> AddAsync(Customer data)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string defaultPassword = "123456";
                string hashedPassword = GetMD5(defaultPassword);

                string sql = @"INSERT INTO Customers
                               (CustomerName, ContactName, Province, Address, Phone, Email, Password, IsLocked)
                               VALUES
                               (@CustomerName, @ContactName, @Province, @Address, @Phone, @Email, @Password, @IsLocked);
                               SELECT CAST(SCOPE_IDENTITY() as int);";

                return await connection.ExecuteScalarAsync<int>(sql, new
                {
                    data.CustomerName,
                    data.ContactName,
                    data.Province,
                    data.Address,
                    data.Phone,
                    data.Email,
                    Password = hashedPassword,
                    IsLocked = data.IsLocked ?? false
                });
            }
        }

        // ================== DELETE ==================
        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM Customers WHERE CustomerID = @id";
                int rows = await connection.ExecuteAsync(sql, new { id });
                return rows > 0;
            }
        }

        // ================== GET ==================
        public async Task<Customer?> GetAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Customers WHERE CustomerID = @id";
                return await connection.QueryFirstOrDefaultAsync<Customer>(sql, new { id });
            }
        }

        // ================== CHECK USED ==================
        public async Task<bool> IsUsedAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT CASE 
                               WHEN EXISTS(SELECT 1 FROM Orders WHERE CustomerID = @id) 
                               THEN 1 ELSE 0 END";

                return await connection.ExecuteScalarAsync<bool>(sql, new { id });
            }
        }

        // ================== LIST ==================
        public async Task<PagedResult<Customer>> ListAsync(PaginationSearchInput input)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT * FROM Customers
                               WHERE (@searchValue = N'') 
                                  OR (CustomerName LIKE @searchValue) 
                                  OR (ContactName LIKE @searchValue)
                               ORDER BY CustomerID
                               OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;

                               SELECT COUNT(*) FROM Customers
                               WHERE (@searchValue = N'') 
                                  OR (CustomerName LIKE @searchValue) 
                                  OR (ContactName LIKE @searchValue);";

                var param = new
                {
                    searchValue = string.IsNullOrEmpty(input.SearchValue) ? "" : $"%{input.SearchValue}%",
                    offset = (input.Page - 1) * input.PageSize,
                    pageSize = input.PageSize
                };

                using (var multi = await connection.QueryMultipleAsync(sql, param))
                {
                    var data = (await multi.ReadAsync<Customer>()).ToList();
                    var count = await multi.ReadFirstAsync<int>();

                    return new PagedResult<Customer>
                    {
                        DataItems = data,
                        RowCount = count,
                        Page = input.Page,
                        PageSize = input.PageSize
                    };
                }
            }
        }

        // ================== UPDATE ==================
        public async Task<bool> UpdateAsync(Customer data)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Customers
                               SET CustomerName = @CustomerName,
                                   ContactName = @ContactName,
                                   Province = @Province,
                                   Address = @Address,
                                   Phone = @Phone,
                                   Email = @Email,
                                   IsLocked = @IsLocked
                               WHERE CustomerID = @CustomerID";

                int rows = await connection.ExecuteAsync(sql, new
                {
                    data.CustomerID,
                    data.CustomerName,
                    data.ContactName,
                    data.Province,
                    data.Address,
                    data.Phone,
                    data.Email,
                    IsLocked = data.IsLocked ?? false
                });

                return rows > 0;
            }
        }

        // ================== CHECK EMAIL ==================
        public async Task<bool> ValidateEmailAsync(string email, int id = 0)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT CASE WHEN EXISTS(
                                SELECT 1 FROM Customers 
                                WHERE Email = @email AND CustomerID <> @id
                               ) THEN 1 ELSE 0 END";

                return await connection.ExecuteScalarAsync<bool>(sql, new { email, id });
            }
        }
        // ================== CHANGE PASSWORD ==================
        public async Task<bool> ChangePasswordAsync(int id, string newPassword)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string hashedPassword = GetMD5(newPassword);

                string sql = @"UPDATE Customers
                       SET Password = @Password
                       WHERE CustomerID = @id";

                int rows = await connection.ExecuteAsync(sql, new
                {
                    id,
                    Password = hashedPassword
                });

                return rows > 0;
            }
        }
    }
}