using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ElectricalShop.Models.Admin
{
    public class Profiler
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được rỗng")]
        public String UserName { get; set; }

        public String Password { get; set; }

        [RegularExpression(@"^.*(?=.{5,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[_!@#$%^&+=]).*$", ErrorMessage = "Mật khẩu dài 5 đến 18 ký tự. Bao gồm ít nhất 1 ký tự hoa, 1 ký tự thường và 1 ký tự đặc biệt (_!@#$%^&+=)")]
        public String NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Xác nhận mật khẩu và mật khẩu không trùng khớp")]
        public String NewPasswordConfirmed { get; set; }

        [Required(ErrorMessage = "Họ tên không được rỗng")]
        public String FullName;

        public String Phone;

        public String Email;
    }
}