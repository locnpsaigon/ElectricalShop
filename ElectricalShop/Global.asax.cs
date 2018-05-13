using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using Newtonsoft.Json;
using ElectricalShop.Models.Principle;

namespace ElectricalShop
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AppSettings.LoadSettings();

            Application["OnlineVisitors"] = 0;
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Application.Lock();
            Application["OnlineVisitors"] = (int)Application["OnlineVisitors"] + 1;
            Application.UnLock();
        }

        protected void Session_End(object sender, EventArgs e)
        {
            Application.Lock();
            Application["OnlineVisitors"] = (int)Application["OnlineVisitors"] - 1;
            Application.UnLock();
        }

        protected void Application_PostAuthenticateRequest()
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                ElectricalShopPrincipleModel serializeModel = JsonConvert.DeserializeObject<ElectricalShopPrincipleModel>(authTicket.UserData);
                ElectricalShopPrinciple principle = new ElectricalShopPrinciple(authTicket.Name);
                principle.UserId = serializeModel.UserId;
                principle.FullName = serializeModel.FullName;
                principle.Roles = serializeModel.Roles;
                HttpContext.Current.User = principle;
            }
        }
    }
}
