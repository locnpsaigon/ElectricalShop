using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ElectricalShop.Models.Admin
{
    public class AddExport
    {
        public string ExportDate { get; set; }
        public string Note { get; set; }
        public List<AddExportRow> AddExportRows { get; set; }

        public AddExport()
        {
            AddExportRows = new List<AddExportRow>();
        }
    }

    public class AddExportRow
    {
        [Required(ErrorMessage = "Vui lòng chọn sản phẩm")]
        public int ProductId { get; set; }
        public string Quantity { get; set; }
        public string ExportPrice { get; set; }
    }
}