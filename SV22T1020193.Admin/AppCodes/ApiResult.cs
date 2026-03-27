namespace SV22T1020193.Admin.AppCodes
{
    /// <summary>
    /// Biểu diễn dữ liệu trả về của các API
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// Biểu diễn dữ liệu trả về của ác API
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public ApiResult(int code,string message = "", int orderID = 0) 
        {
            Code = code;
            Message = message;
        }
        /// <summary>
        /// Mã kết quả trả về (*0 tức là lỗi hoặc k thành công)
        /// </summary>
        public int Code {  get; set; }
        /// <summary>
        /// Thông báo lỗi (nếu có)
        /// </summary>
        public string Message { get; set; } = "";
    }
}
