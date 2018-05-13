using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ElectricalShop.Models.DataAccess;
using ElectricalShop.Models.Principle;
using ElectricalShop.Common;


namespace ElectricalShop.Controllers.Admin
{
    public class DashboardController : BaseController
    {
        DBContext db = new DBContext();

        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult EximStat()
        {
            ViewBag.BrandSelectOptions = DropDownListOption.GetBrandOptions();
            ViewBag.CategorySelectOptions = DropDownListOption.GetCategoryOptions();
            return View();
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public JsonResult GetEximStat(string date1, string date2, int cat = 0, int brand = 0)
        {
            try
            {
                DateTime d1 = DateTime.Now;
                DateTime d2 = DateTime.Now;

                // Parse date range
                Boolean isValidDateRange =
                    DateTime.TryParseExact(date1 + " 00:00:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out d1) &&
                    DateTime.TryParseExact(date2 + " 23:59:59", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out d2);

                if (isValidDateRange)
                {
                    // opening stock import
                    var openingStockImports = db.Imports.Where(im => im.ImportDate < d1)
                        .GroupBy(im => im.ProductId)
                        .Select(g => new
                        {
                            ProductId = g.FirstOrDefault().ProductId,
                            Quantity = g.Sum(s => s.Quantity),
                            Amount = g.Sum(s => s.Quantity * s.ImportPrice)
                        });

                    // opening stock export
                    var openingStockExports = db.Exports.Where(ex => ex.ExportDate < d1) 
                        .GroupBy(ex => ex.ProductId)
                        .Select(g => new
                        {
                            ProductId = g.FirstOrDefault().ProductId,
                            Quantity = g.Sum(s => s.Quantity),
                            Amount = g.Sum(s => s.Quantity * s.ExportPrice)
                        });

                    // stock import in period
                    var stockImports = db.Imports.Where(im => im.ImportDate >= d1 && im.ImportDate <= d2)
                        .GroupBy(im => im.ProductId)
                        .Select(g => new
                        {
                            ProductId = g.FirstOrDefault().ProductId,
                            Quantity = g.Sum(s => s.Quantity),
                            Amount = g.Sum(s => s.Quantity * s.ImportPrice)
                        });

                    // stock export in period
                    var stockExports = db.Exports.Where(ex => ex.ExportDate >= d1 && ex.ExportDate <= d2)
                        .GroupBy(ex => ex.ProductId)
                        .Select(g => new
                        {
                            ProductId = g.FirstOrDefault().ProductId,
                            Quantity = g.Sum(s => s.Quantity),
                            Amount = g.Sum(s => s.Quantity * s.ExportPrice)
                        });

                    var eximData = from p1 in db.Products
                                   join r1 in openingStockImports on p1.ProductId equals r1.ProductId into j1
                                   join r2 in openingStockExports on p1.ProductId equals r2.ProductId into j2
                                   join r3 in stockImports on p1.ProductId equals r3.ProductId into j3
                                   join r4 in stockExports on p1.ProductId equals r4.ProductId into j4
                                   from t1 in j1.DefaultIfEmpty() // nhap dau ky
                                   from t2 in j2.DefaultIfEmpty() // xuat dau ky
                                   from t3 in j3.DefaultIfEmpty() // nhap trong ky
                                   from t4 in j4.DefaultIfEmpty() // xuat trong ky
                                   join c1 in db.Categories on p1.CategoryId equals c1.CategoryId into j5
                                   join b1 in db.Brands on p1.BrandId equals b1.BrandId into j6
                                   from t5 in j5.DefaultIfEmpty()
                                   from t6 in j6.DefaultIfEmpty()
                                   select new
                                   {
                                       ProductId = p1.ProductId,
                                       SKU = p1.SKU,
                                       ProductName = p1.ProductName,
                                       QuantityPerUnit = p1.QuantityPerUnit,
                                       CategoryId = p1.CategoryId,
                                       CategoryName = t5.CategoryName,
                                       BrandId = p1.BrandId,
                                       BrandName = t6.BrandName,
                                       OpeningStockQuantity = (t1 == null ? 0 : t1.Quantity) - (t2 == null ? 0 : t2.Quantity),
                                       OpeningStockAmount = (t1 == null ? 0 : t1.Amount) - (t2 == null ? 0 : t2.Amount),
                                       ImportStockQuantity = t3 == null ? 0 : t3.Quantity,
                                       ImportStockAmount = t3 == null ? 0 : t3.Amount,
                                       ExportStockQuantity = t4 == null ? 0 : t4.Quantity,
                                       ExportStockAmount = t4 == null ? 0 : t4.Amount,
                                       ClosingStockQuantity = (t1 == null ? 0 : t1.Quantity) + (t3 == null ? 0 : t3.Quantity) - (t2 == null ? 0 : t2.Quantity) - (t4 == null ? 0 : t4.Quantity),
                                       ClosingStockAmount = 
                                        ((t1 == null ? 0 : t1.Amount) - (t2 == null ? 0 : t2.Amount) + (t3 == null ? 0 : t3.Amount) /  // Giá trị tồn đầu kỳ + trong kỳ
                                        (((t1 == null ? 0 : t1.Quantity) - (t2 == null ? 0 : t2.Quantity) + (t3 == null ? 0 : t3.Quantity)) <= 0 ? 1 : ((t1 == null ? 0 : t1.Quantity) - (t2 == null ? 0 : t2.Quantity) + (t3 == null ? 0 : t3.Quantity)))) * // Số lượng tồn đầu kỳ + trong kỳ
                                        ((t1 == null ? 0 : t1.Quantity) + (t3 == null ? 0 : t3.Quantity) - (t2 == null ? 0 : t2.Quantity) - (t4 == null ? 0 : t4.Quantity)) // Số lượng tồn cuối kỳ

                                   };

                    // filter by category
                    if (cat > 0)
                    {
                        eximData = eximData.Where(x => x.CategoryId == cat);
                    }

                    // filter by brand
                    if (brand > 0)
                    {
                        eximData = eximData.Where(x => x.BrandId == brand);
                    }

                    // sorting
                    eximData = eximData.OrderBy(x => x.CategoryName).ThenBy(x => x.BrandName).ThenBy(x => x.ProductName);

                    return Json(new
                    {
                        Success = true,
                        Message = "Get import/export report data success",
                        EximData = eximData.ToList()
                    });

                }
                else
                {
                    return Json(new { Success = false, Message = "Thời gian tra cứu không hợp lệ!" });
                }
                
            }
            catch(Exception ex)
            {
                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Stock - GetEximStat()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);

                return Json(new { Success = false, Message = ex.Message });
            }
        }
    }
}