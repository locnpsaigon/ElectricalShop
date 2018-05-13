using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElectricalShop.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Oops()
        {
            return View();
        }
    }
}