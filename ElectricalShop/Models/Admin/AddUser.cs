using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ElectricalShop.Models.Admin
{
    public class AddUser
    {
        [Required(ErrorMessage = "Tên đăng nhập không được rỗng")]
        [RegularExpression(@"^[a-z0-9_-]{2,15}$", ErrorMessage = "Tên tài khoản chỉ gồm chữ và số, gạch ngang và gạch dưới")]
        public String UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được rỗng")]
        [RegularExpression(@"^.*(?=.{5,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[_!@#$%^&+=]).*$", ErrorMessage = "Mật khẩu dài 5 đến 18 ký tự. Bao gồm ít nhất 1 ký tự hoa, 1 ký tự thường và 1 ký tự đặc biệt (_!@#$%^&+=)")]
        public String Password { get; set; }

        [Compare("Password", ErrorMessage = "Xác nhận mật khẩu và mật khẩu không trùng khớp")]
        [RegularExpression(@"^.*(?=.{5,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[_!@#$%^&+=]).*$", ErrorMessage = "Mật khẩu dài 5 đến 18 ký tự. Bao gồm ít nhất 1 ký tự hoa, 1 ký tự thường và 1 ký tự đặc biệt (_!@#$%^&+=)")]
        public String PasswordConfirmed { get; set; }

        [Required(ErrorMessage = "Họ tên không được rỗng")]
        public String FullName { get; set; }

        public String Phone { get; set; }

        public String Email { get; set; }

        public bool IsActive { get; set; }

        public List<RoleCheckBox> RoleCheckBoxes { get; set; }

        public AddUser()
        {
            RoleCheckBoxes = new List<RoleCheckBox>();
        }
    }
}