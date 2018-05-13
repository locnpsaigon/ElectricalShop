using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectricalShop.Models.Admin
{
    public class RoleCheckBox
    {
        public int RoleId { get; set; }
        public String RoleName { get; set; }
        public bool Selected { get; set; }
    }
}