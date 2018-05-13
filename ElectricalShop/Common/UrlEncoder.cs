using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElectricalShop.Common
{
    public class UrlEncoder
    {
        public static string ToFriendlyUrl(UrlHelper helper, string title)
        {
            var url = title.Trim();
            url = url.Replace("  ", " ").Replace(" - ", " ").Replace(" ", "-").Replace(",", "").Replace("...", "");
            return url;
        }
    }
}