using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ElectricalShop.Models.Admin
{
    public class EditCategory
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Tên chủng loại không được rỗng")]
        public String CategoryName { get; set; }

        public String Description { get; set; }

        public int? ParrentId { get; set; }

        public IList<SelectListItem> SelectCategoryItems { get; set; }
    }
}