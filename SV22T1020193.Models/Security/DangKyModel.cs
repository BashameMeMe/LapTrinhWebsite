using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1020193.Models.Security
{
    public class DangKyModel
   {
     [Required(ErrorMessage = "Tên khách hàng không được để trống")]
    public string CustomerName { get; set; } = "";

    public string? ContactName { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn Tỉnh/Thành phố")]
    public string Province { get; set; } = "";

    public string? Address { get; set; }

    public string? Phone { get; set; }

    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu")]
    [Compare("Password", ErrorMessage = "Mật khẩu nhập lại không khớp")]
    public string ConfirmPassword { get; set; } = "";
    }
}
