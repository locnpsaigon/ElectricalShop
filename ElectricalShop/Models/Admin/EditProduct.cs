using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ElectricalShop.Models.Admin
{
    public class EditProduct
    {
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Tên sản mặt hàng không được rỗng")]
        public string ProductName { get; set; }

        public string Description { get; set; }

        public int? BrandId { get; set; }

        public int? CategoryId { get; set; }

        public string SKU { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập đơn vị tính")]
        public string QuantityPerUnit { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá")]
        public string Price { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giảm giá")]
        public string Discount { get; set; }

        public IList<SelectListItem> CategorySelectOptions;
        public IList<SelectListItem> BrandSelectOptions;
    }
}