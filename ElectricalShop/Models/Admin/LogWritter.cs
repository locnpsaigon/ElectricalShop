using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ElectricalShop.Models.DataAccess;

namespace ElectricalShop.Models.Admin
{
    public class LogWritter
    {
        public static bool WriteLog(Log log)
        {
            try
            {
                using(DBContext db = new DBContext())
                {
                    db.Logs.Add(log);
                    db.SaveChanges();
                    return true;
                }
            }
            catch 
            {
                return false;
            }
        }
    }
}