using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ElectricalShop.Models.Admin
{
    public class AddImport
    {
        public string ImportDate { get; set; }
        public string Note { get; set; }
        public List<AddImportRow> AddImportRows { get; set; }

        public AddImport()
        {
            AddImportRows = new List<AddImportRow>();
        }
    }

    public class AddImportRow
    {
        [Required(ErrorMessage = "Vui lòng chọn nhà cung cấp")]
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn sản phẩm")]
        public int ProductId { get; set; }

        public string Quantity { get; set; }

        public string ImportPrice { get; set; }
    }
}