using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ElectricalShop.Models.DataAccess;
using ElectricalShop.Common;

namespace ElectricalShop.Controllers
{
    public class HomeController : Controller
    {
        DBContext db = new DBContext();

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Admin");
        }
    }
}