using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ElectricalShop.Common;
using ElectricalShop.Models.DataAccess;
using ElectricalShop.Models.Principle;

namespace ElectricalShop.Controllers.Admin
{
    public class LogController : BaseController
    {

        DBContext db = new DBContext();

        // GET: Log
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public JsonResult Delete(string ids)
        {
            try
            {
                var logIds = ids.Split(',').ToList();
                var logs = db.Logs.Where(x => logIds.Contains(x.LogId.ToString())).ToList();
                db.Logs.RemoveRange(logs);
                db.SaveChanges();
                return Json(new { Success = true, Message = "Xóa logs thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public JsonResult FilterLogs(string filterText = "", string date1 = "", string date2 = "", int page = 1)
        {
            try
            {
                DateTime d1 = DateTime.Now;
                DateTime d2 = DateTime.Now;

                // Try to parse date range
                Boolean isValidDateRange =
                    DateTime.TryParseExact(date1 + " 00:00:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out d1) &&
                    DateTime.TryParseExact(date2 + " 23:59:59", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out d2);

                if (isValidDateRange && d2.Subtract(d1).Days <= 30)
                {
                    // Filter logs
                    var logs = db.Logs.Where(x => x.LogDate >= d1 && x.LogDate <= d2);
                    if (!string.IsNullOrWhiteSpace(filterText))
                    {
                        logs = logs.Where(x => x.Action.Contains(filterText.Trim()) || x.Message.Contains(filterText.Trim()));
                    }
                    // Sort logs
                    logs = logs.OrderByDescending(x => x.LogDate);

                    // Paging logs
                    var pager = new ListPager(logs.ToList(), page, AppSettings.DEFAULT_PAGE_SIZE);

                    return Json(new
                    {
                        Success = true,
                        Message = "Filter roles success",
                        RowCount = pager.RowCount,
                        PageIndex = pager.PageIndex,
                        PageSize = pager.PageSize,
                        PageTotal = pager.PageTotal,
                        Logs = pager.PagedData
                    });
                }
                else
                {
                    return Json(new { Success = false, Message = "Khoảng thời gian tra cứu không quá 30 ngày!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }
    }
}