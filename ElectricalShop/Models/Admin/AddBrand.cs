﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ElectricalShop.Models.Admin
{
    public class AddBrand
    {
        [Required(ErrorMessage = "Tên thương hiệu không được rỗng")]
        public String BrandName { get; set; }

        public String Description { get; set; }
    }
}