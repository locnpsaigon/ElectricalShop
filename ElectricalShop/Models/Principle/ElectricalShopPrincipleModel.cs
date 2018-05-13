using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectricalShop.Models.Principle
{
    public class ElectricalShopPrincipleModel
    {
        public int UserId { get; set; }

        public string FullName { get; set; }

        public String[] Roles { get; set; }
    }
}