using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ElectricalShop.Models.Admin
{
    public class EditRole
    {
        [Key]
        public int RoleId { get; set; }
        [Required(ErrorMessage = "Chức danh quản trị viên không được rỗng!")]
        public String RoleName { get; set; }
        public String Description { get; set; }
    }
}