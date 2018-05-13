using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ElectricalShop.Models.Admin
{
    public class EditExport
    {
        [Key]
        public int ExportId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày xuất kho")]
        public string ExportDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn sản phẩm xuất kho")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng xuất")]
        public string Quantity { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá xuất")]
        public string ExportPrice { get; set; }

        public string Note { get; set; }

        public List<SelectListItem> ProductSelectOptions { get; set; }

        // Constructors
        public EditExport()
        {
            ProductSelectOptions = new List<SelectListItem>();
        }
    }
}