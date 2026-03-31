using System.Security.Cryptography;
using System.Text;

namespace SV22T1020193.Admin
{
    /// <summary>
    /// Lớp cung cấp các hàm tiện ích sử dụng cho mã hóa
    /// </summary>
    public static class CryptHelper
    {
        /// <summary>
        /// Mã hóa MD5 một chuỗi
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string HashMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
   //nguyên tắt chung:
        //Người sử dụng cung cấp thông tin để kiểm tra(Username+password/Sinh trắc học) AuthID/OpenID
        //Hệ thộng kiểm tra thông tin có hợp lệ không?
        //Nếu hợp lệ cấp cho người dùng chứng chỉ/Chứng nhận(Principal) và giao cho user(cookie)
        //Phía clinent lưu trữ cookie, và đính kèm cookie(Trong phần header) mooix khi cos requett len server
        //Phia server dua vao cookie de kiem tra user co hop le k
        // Authentication
        // Authorization
        // Trong ASP.NET Core, muốn sử dụng Auth thì phải đăng ký.
        // Để sử udngj cơ chế Authorization đối với các Controller hoặc Action, đặt chỉ thị sau ở phía trước:
        // [Autherization]
        // Dùng [AllowAnonymous] trong acction (sử dụng ở acction thì quyền sẽ cao hơn và bỏ qua [Autherization]
        //Trong action haowcj View, thông qua thuộc tính user để lấy được "Giấy chứng nhận" đã cấp cho client
        //
}
