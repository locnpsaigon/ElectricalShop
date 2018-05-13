using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ElectricalShop.Models.Admin
{
    public class EditSupplier
    {
        [Key]
        public int SupplierId { get; set; }
        [Required(ErrorMessage = "Tên nhà cung cấp không được rỗng")]
        public string SupplierName { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
    }
}