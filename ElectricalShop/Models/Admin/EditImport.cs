using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ElectricalShop.Models.Admin
{
    public class EditImport
    {
        [Key]
        public int ImportId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày nhập kho")]
        public string ImportDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nhà cung cấp")]
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn sản phẩm nhập kho")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        public string Quantity { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá")]
        public string ImportPrice { get; set; }

        public string Note { get; set; }

        public List<SelectListItem> SupplierSelectOptions { get; set; }
        public List<SelectListItem> ProductSelectOptions { get; set; }

        // Constructors
        public EditImport()
        {
            ProductSelectOptions = new List<SelectListItem>();
            SupplierSelectOptions = new List<SelectListItem>();
        }
    }
}