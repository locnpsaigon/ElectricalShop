using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ElectricalShop.Models.Principle;

namespace ElectricalShop.Controllers.Admin

{
    public class BaseController : Controller
    {
        protected virtual new ElectricalShopPrinciple User
        {
            get { return HttpContext.User as ElectricalShopPrinciple; }
        }

        protected string GetRequestedIP()
        {
            return Request.ServerVariables["REMOTE_ADDR"];
        }

        protected string GetLogonUserName()
        {
            return User == null ? "Guest" : User.Identity.Name;
        }
    }
}