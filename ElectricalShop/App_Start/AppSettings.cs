using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace ElectricalShop
{
    public class AppSettings
    {
        public static int DEFAULT_PAGE_SIZE = 25;
        public static string IMAGE_UPLOAD_PATH = "/Images/";
        public static string FILE_MANAGER_ICONS_PATH = "/Scripts/vendors/filemanager/images/fileicons/";

        public static void LoadSettings()
        {
            try
            {
                Int32.TryParse(ConfigurationManager.AppSettings["DEFAULT_PAGE_SIZE"], out DEFAULT_PAGE_SIZE);
            }
            catch { }
        }
    }
}