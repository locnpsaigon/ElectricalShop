using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ElectricalShop.Models.Admin;
using ElectricalShop.Models.DataAccess;
using ElectricalShop.Models.Principle;
using ElectricalShop.Common;

namespace ElectricalShop.Controllers.Admin
{
    public class StockController : BaseController
    {
        DBContext db = new DBContext();

        // GET: Stock
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult Index()
        {
            ViewBag.BrandSelectOptions = DropDownListOption.GetBrandOptions();
            ViewBag.CategorySelectOptions = DropDownListOption.GetCategoryOptions();
            return View();
        }

        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult ImportHistory()
        {
            ViewBag.BrandSelectOptions = DropDownListOption.GetBrandOptions();
            ViewBag.CategorySelectOptions = DropDownListOption.GetCategoryOptions();
            ViewBag.SupplierSelectOptions = DropDownListOption.GetSupplierOptions();
            return View();
        }

        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult ExportHistory()
        {
            ViewBag.BrandSelectOptions = DropDownListOption.GetBrandOptions();
            ViewBag.CategorySelectOptions = DropDownListOption.GetCategoryOptions();
            return View();
        }

        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult Import()
        {
            var model = new AddImport();

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult Import(AddImport model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DateTime importDate = DateTime.Now;
                    if (DateTime.TryParseExact(model.ImportDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out importDate))
                    {
                        bool success = true;
                        foreach (var row in model.AddImportRows)
                        {
                            var importProduct = db.Products.Where(p => p.ProductId == row.ProductId).FirstOrDefault();
                            if (importProduct != null)
                            {
                                int quantity = 0;
                                decimal importPrice = 0;
                                if (int.TryParse(row.Quantity.Replace(",", ""), out quantity) &&
                                    decimal.TryParse(row.ImportPrice.Replace(",", ""), out importPrice))
                                {
                                    // add new import
                                    var import = new Import();
                                    import.ImportDate = importDate;
                                    import.SupplierId = row.SupplierId;
                                    import.ProductId = row.ProductId;
                                    import.Quantity = quantity;
                                    import.ImportPrice = importPrice;
                                    import.Note = model.Note;
                                    import.CreationDate = DateTime.Now;
                                    import.CreationUser = GetLogonUserName();
                                    db.Imports.Add(import);
                                    importProduct.UnitInStock += quantity;
                                }
                                else
                                {
                                    success = false;
                                    ModelState.AddModelError("", "Số lượng hoặc giá nhập sản phẩm " + importProduct.ProductName + " không hợp lệ!");
                                    break;
                                }
                            }
                        }
                        if (success)
                        {
                            db.SaveChanges();
                            return RedirectToAction("ImportHistory");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Ngày nhập hàng không hợp lệ!");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Stock - Import()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);
            }
            return View(model);
        }

        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult EditImport(int id)
        {
            var model = new EditImport();
            try
            {
                var import = db.Imports.Where(im => im.ImportId == id).FirstOrDefault();
                if (import != null)
                {
                    model.ImportId = import.ImportId;
                    model.ImportDate = import.ImportDate.ToString("dd/MM/yyyy");
                    model.SupplierId = import.SupplierId;
                    model.ProductId = import.ProductId;
                    model.Quantity = import.Quantity.ToString("#,##0");
                    model.ImportPrice = import.ImportPrice.ToString("#,##0");
                    model.Note = import.Note;
                    model.SupplierSelectOptions = DropDownListOption.GetSupplierOptions(import.SupplierId);
                    model.ProductSelectOptions = DropDownListOption.GetProductOptions(import.ProductId);
                }
                else
                {
                    ModelState.AddModelError("", "Không tìm thấy thông tin nhập kho!");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Stock - EditImport()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult EditImport(EditImport model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var import = db.Imports.Where(im => im.ImportId == model.ImportId).FirstOrDefault();
                    if (import != null)
                    {
                        int quantity = 0;
                        decimal importPrice = 0;
                        DateTime importDate = DateTime.Now;

                        // Validate user input data
                        if (!DateTime.TryParseExact(model.ImportDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out importDate))
                        {
                            ModelState.AddModelError("", "Ngày nhập không hợp lệ!");
                            return View(model);
                        }

                        if (!int.TryParse(model.Quantity.Replace(",", ""), out quantity))
                        {
                            ModelState.AddModelError("", "Số lượng nhập không hợp lệ!");
                            return View(model);
                        }

                        if (!decimal.TryParse(model.ImportPrice.Replace(",", ""), out importPrice))
                        {
                            ModelState.AddModelError("", "Giá nhập không hợp lệ!");
                            return View(model);
                        }

                        // update unit instock
                        var currentProduct = db.Products.Where(p => p.ProductId == import.ProductId).FirstOrDefault();
                        if (currentProduct != null)
                        {
                            currentProduct.UnitInStock -= import.Quantity;
                        }
                        if (currentProduct.ProductId == model.ProductId)
                        {
                            currentProduct.UnitInStock += quantity;
                        }
                        else
                        {
                            var updatedProduct = db.Products.Where(p => p.ProductId == model.ProductId).FirstOrDefault();
                            if (updatedProduct != null)
                            {
                                updatedProduct.UnitInStock += quantity;
                            }
                        }

                        // update import info
                        import.ImportDate = importDate;
                        import.SupplierId = model.SupplierId;
                        import.ProductId = model.ProductId;
                        import.Quantity = quantity;
                        import.ImportPrice = importPrice;
                        import.ProductId = model.ProductId;
                        import.Note = model.Note;

                        db.SaveChanges();
                        return RedirectToAction("ImportHistory");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Không tìm thấy thông tin nhập kho!");
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Thông tin nhập kho không hợp lệ!");
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Stock - EditImport()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);
            }
            finally
            {
                model.ProductSelectOptions = DropDownListOption.GetProductOptions(model.ProductId);
                model.SupplierSelectOptions = DropDownListOption.GetSupplierOptions(model.SupplierId);
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public JsonResult DeleteImport(int id)
        {
            try
            {
                var import = db.Imports.Where(im => im.ImportId == id).FirstOrDefault();
                if (import != null)
                {
                    // update unit in stock
                    var productImport = db.Products.Where(p => p.ProductId == import.ProductId).FirstOrDefault();
                    if (productImport != null)
                    {
                        productImport.UnitInStock -= import.Quantity;
                    }
                    db.Imports.Remove(import);
                    db.SaveChanges();
                }
                return Json(new { Success = true, Message = "Xóa nhập hàng thành công!" });
            }
            catch(Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult Export()
        {
            var model = new AddExport();
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult Export(AddExport model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DateTime importDate = DateTime.Now;
                    if (DateTime.TryParseExact(model.ExportDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out importDate))
                    {
                        bool success = true;
                        foreach (var row in model.AddExportRows)
                        {
                            var exportProduct = db.Products.Where(p => p.ProductId == row.ProductId).FirstOrDefault();
                            if (exportProduct != null)
                            {
                                int quantity = 0;
                                decimal exportPrice = 0;
                                if (int.TryParse(row.Quantity.Replace(",", ""), out quantity) &&
                                    decimal.TryParse(row.ExportPrice.Replace(",", ""), out exportPrice))
                                {
                                    // add new export
                                    var export = new Export();
                                    export.ExportDate = importDate;
                                    export.ProductId = row.ProductId;
                                    export.Quantity = quantity;
                                    export.ExportPrice = exportPrice;
                                    export.Note = model.Note;
                                    export.CreationDate = DateTime.Now;
                                    export.CreationUser = GetLogonUserName();
                                    db.Exports.Add(export);
                                    exportProduct.UnitInStock -= quantity;
                                }
                                else
                                {
                                    success = false;
                                    ModelState.AddModelError("", "Số lượng hoặc giá nhập sản phẩm " + exportProduct.ProductName + " không hợp lệ!");
                                    break;
                                }
                            }
                        }
                        if (success)
                        {
                            db.SaveChanges();
                            return RedirectToAction("ExportHistory");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Ngày nhập hàng không hợp lệ!");
                    }
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Stock - Export()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);
            }
            return View(model);
        }

        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult EditExport(int id)
        {
            var model = new EditExport();
            try
            {
                var export = db.Exports.Where(im => im.ExportId == id).FirstOrDefault();
                if (export != null)
                {
                    model.ExportId = export.ExportId;
                    model.ExportDate = export.ExportDate.ToString("dd/MM/yyyy");
                    model.ProductId = export.ProductId;
                    model.Quantity = export.Quantity.ToString("#,##0");
                    model.ExportPrice = export.ExportPrice.ToString("#,##0");
                    model.Note = export.Note;
                    model.ProductSelectOptions = DropDownListOption.GetProductOptions(export.ProductId);
                }
                else
                {
                    ModelState.AddModelError("", "Không tìm thấy thông tin xuất kho!");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Stock - EditExport()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult EditExport(EditExport model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var export = db.Exports.Where(im => im.ExportId == model.ExportId).FirstOrDefault();
                    if (export != null)
                    {
                        int quantity = 0;
                        decimal exportPrice = 0;
                        DateTime exportDate = DateTime.Now;

                        // Validate user input data
                        if (!DateTime.TryParseExact(model.ExportDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out exportDate))
                        {
                            ModelState.AddModelError("", "Ngày xuất không hợp lệ!");
                            return View(model);
                        }

                        if (!int.TryParse(model.Quantity.Replace(",", ""), out quantity))
                        {
                            ModelState.AddModelError("", "Số lượng xuất không hợp lệ!");
                            return View(model);
                        }

                        if (!decimal.TryParse(model.ExportPrice.Replace(",", ""), out exportPrice))
                        {
                            ModelState.AddModelError("", "Giá xuất không hợp lệ!");
                            return View(model);
                        }

                        // update product unit in stock
                        var currentProduct = db.Products.Where(p => p.ProductId == export.ProductId).FirstOrDefault();
                        if (currentProduct != null)
                        {
                            currentProduct.UnitInStock -= export.Quantity;
                        }
                        if (currentProduct.ProductId == model.ProductId)
                        {
                            currentProduct.UnitInStock += quantity;
                        }
                        else
                        {
                            var updatedProduct = db.Products.Where(p => p.ProductId == model.ProductId).FirstOrDefault();
                            if (updatedProduct != null)
                            {
                                updatedProduct.UnitInStock += quantity;
                            }
                        }

                        // update import info
                        export.ExportDate = exportDate;
                        export.ProductId = model.ProductId;
                        export.Quantity = quantity;
                        export.ExportPrice = exportPrice;
                        export.Note = model.Note;

                        db.SaveChanges();
                        return RedirectToAction("ExportHistory");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Không tìm thấy thông tin xuất kho!");
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Thông tin xuất kho không hợp lệ!");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Stock - EditExport()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);
            }
            finally
            {
                model.ProductSelectOptions = DropDownListOption.GetProductOptions(model.ProductId);
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public JsonResult DeleteExport(int id)
        {
            try
            {
                var export = db.Exports.Where(im => im.ExportId == id).FirstOrDefault();
                if (export != null)
                {
                    // update unit in stock
                    var productExport = db.Products.Where(pex => pex.ProductId == export.ProductId).FirstOrDefault();
                    if (productExport != null)
                    {
                        productExport.UnitInStock += export.Quantity;
                    }
                    db.Exports.Remove(export);
                    db.SaveChanges();
                }
                return Json(new { Success = true, Message = "Xóa xuất hàng thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public JsonResult FilterImports(string date1, string date2, string searchText = "", int cat = 0, int brand = 0, int supplier = 0)
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
                    if (d2.Subtract(d1).Days <= 60)
                    {
                        // filter by date range
                        var importQuery = from t1 in db.Imports
                                          join t2 in db.Suppliers on t1.SupplierId equals t2.SupplierId into j1
                                          join t3 in db.Products on t1.ProductId equals t3.ProductId into j2
                                          from t4 in j1.DefaultIfEmpty()
                                          from t5 in j2.DefaultIfEmpty()
                                          join t6 in db.Categories on t5.CategoryId equals t6.CategoryId into j3
                                          join t7 in db.Brands on t5.BrandId equals t7.BrandId into j4
                                          from t8 in j3.DefaultIfEmpty()
                                          from t9 in j4.DefaultIfEmpty()
                                          where t1.ImportDate >= d1 && t1.ImportDate <= d2
                                          select new
                                          {
                                              ImportId = t1.ImportId,
                                              ImportDate = t1.ImportDate,
                                              SupplierId = t4.SupplierId,
                                              SupplierName = t4.SupplierName,
                                              ProductId = t1.ProductId,
                                              ProductName = t9.BrandName + " -" + t5.ProductName,
                                              ProductNameUnsign = t9.BrandName + " -" + t5.ProductNameUnsign,
                                              BrandId = t9.BrandId,
                                              BrandName = t9.BrandName,
                                              CategoryId = t5.CategoryId,
                                              CategoryName = t8.CategoryName,
                                              SKU = t5.SKU,
                                              Quantity = t1.Quantity,
                                              ImportPrice = t1.ImportPrice,
                                              Note = t1.Note,
                                              CreationDate = t1.CreationDate,
                                              CreationUser = t1.CreationUser
                                          };

                        // filter by category
                        if (cat > 0)
                        {
                            importQuery = importQuery.Where(im => im.CategoryId == cat);
                        }

                        // filter by brand
                        if (brand > 0)
                        {
                            importQuery = importQuery.Where(im => im.BrandId == brand);
                        }

                        // filter by supplier
                        if (supplier > 0)
                        {
                            importQuery = importQuery.Where(im => im.SupplierId == supplier);
                        }

                        // filter by search text
                        if (!string.IsNullOrWhiteSpace(searchText))
                        {
                            var searchTextUnsign = StringExtensions.convertToUnSign(searchText.Trim().ToLower());
                            importQuery = importQuery.Where(im =>
                                im.BrandName.Contains(searchText) ||
                                im.ProductNameUnsign.Contains(searchTextUnsign) ||
                                im.CategoryName.Contains(searchText));
                        }

                        // sorting
                        importQuery = importQuery.OrderByDescending(im => im.ImportDate).ThenBy(im => im.ProductName);

                        // paging
                        return Json(new
                        {
                            Success = true,
                            Message = "Filter imports success",
                            Imports = importQuery.ToList()
                        });
                    }
                    else
                    {
                        return Json(new { Success = false, Message = "Thời gian tra cứu không được quá 60 ngày!" });
                    }
                }
                else
                {
                    return Json(new { Success = false, Message = "Thời gian tra cứu không hợp lệ!" });
                }
            }
            catch (Exception ex)
            {
                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Stock - FilterImports()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);

                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public JsonResult FilterExports(string date1, string date2, string searchText = "", int cat = 0, int brand = 0)
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
                    if (d2.Subtract(d1).Days <= 60)
                    {
                        // filter by date range
                        var exportQuery = from t1 in db.Exports
                                          join t2 in db.Products on t1.ProductId equals t2.ProductId
                                          join t3 in db.Categories on t2.CategoryId equals t3.CategoryId into j1
                                          join t4 in db.Brands on t2.BrandId equals t4.BrandId into j2
                                          from t5 in j1.DefaultIfEmpty()
                                          from t6 in j2.DefaultIfEmpty()
                                          where t1.ExportDate >= d1 && t1.ExportDate <= d2
                                          select new
                                          {
                                              ExportId = t1.ExportId,
                                              ExportDate = t1.ExportDate,
                                              ProductId = t1.ProductId,
                                              ProductName = t6.BrandName + " -" + t2.ProductName,
                                              ProductNameUnsign = t6.BrandName + " -" + t2.ProductNameUnsign,
                                              BrandId = t6.BrandId,
                                              BrandName = t6.BrandName,
                                              CategoryId = t5.CategoryId,
                                              CategoryName = t5.CategoryName,
                                              SKU = t2.SKU,
                                              Quantity = t1.Quantity,
                                              ExportPrice = t1.ExportPrice,
                                              Note = t1.Note,
                                              CreationDate = t1.CreationDate,
                                              CreationUser = t1.CreationUser
                                          };

                        // filter by category
                        if (cat > 0)
                        {
                            exportQuery = exportQuery.Where(ex => ex.CategoryId == cat);
                        }

                        // filter by brand
                        if (brand > 0)
                        {
                            exportQuery = exportQuery.Where(ex => ex.BrandId == brand);
                        }

                        // filter by search text
                        if (!string.IsNullOrWhiteSpace(searchText))
                        {
                            var searchTextUnsign = StringExtensions.convertToUnSign(searchText.Trim().ToLower());
                            exportQuery = exportQuery.Where(ex =>
                                ex.BrandName.Contains(searchText) ||
                                ex.ProductNameUnsign.Contains(searchTextUnsign) ||
                                ex.CategoryName.Contains(searchText));
                        }

                        // sorting
                        exportQuery = exportQuery.OrderByDescending(ex => ex.ExportDate).ThenBy(ex => ex.ProductName);

                        // paging
                        return Json(new
                        {
                            Success = true,
                            Message = "Filter exports success",
                            Exports = exportQuery.ToList()
                        });
                    }
                    else
                    {
                        return Json(new { Success = false, Message = "Thời gian tra cứu không được quá 60 ngày!" });
                    }
                }
                else
                {
                    return Json(new { Success = false, Message = "Thời gian tra cứu không hợp lệ!" });
                }
            }
            catch (Exception ex)
            {
                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Stock - FilterExports()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);

                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public JsonResult FilterProductsInStock(string searchText = "", int cat = 0, int brand = 0, string sortName = "name", string sortType = "asc", int page = 1)
        {
            try
            {
                // filter products
                var productQuery = from p in db.Products select p;
                if (brand > 0)
                {
                    productQuery = productQuery.Where(p => p.BrandId == brand);
                }
                if (cat > 0)
                {
                    productQuery = productQuery.Where(p => p.CategoryId == cat);
                }
                if (string.IsNullOrWhiteSpace(searchText) == false)
                {
                    searchText = StringExtensions.convertToUnSign(searchText).ToLower();
                    productQuery = productQuery.Where(p =>
                        p.ProductNameUnsign.Contains(searchText) ||
                        p.DescriptionUnsign.Contains(searchText) ||
                        p.SKU.Contains(searchText));
                }

                

                // join tables
                var products = from p in productQuery
                               join t1 in db.Brands on p.BrandId equals t1.BrandId into j1
                               join t2 in db.Categories on p.CategoryId equals t2.CategoryId into j2
                               from t3 in j1.DefaultIfEmpty()
                               from t4 in j2.DefaultIfEmpty()
                               orderby p.ProductName
                               select new
                               {
                                   ProductId = p.ProductId,
                                   ProductName = p.ProductName,
                                   Description = p.Description,
                                   BrandId = p.BrandId,
                                   BrandName = t3.BrandName,
                                   CategoryId = p.CategoryId,
                                   CategoryName = t4.CategoryName,
                                   SKU = p.SKU,
                                   QuantityPerUnit = p.QuantityPerUnit,
                                   Price = p.Price,
                                   Discount = p.Discount,
                                   UnitInStock = p.UnitInStock,
                                   CreationDate = p.CreationDate,
                                   CreationUser = p.CreationUser,
                                   LastUpdate = p.LastUpdate,
                                   LastUpdateUser = p.LastUpdateUser,
                                   Status = p.Status
                               };

                // sorting
                switch (sortName)
                {
                    case "name":
                        if (sortType.CompareTo("asc") == 0)
                        {
                            products = products.OrderBy(p => p.ProductName);
                        }
                        else
                        {
                            products = products.OrderByDescending(p => p.ProductName);
                        }
                        break;
                    case "quantity":
                        if (sortType.CompareTo("asc") == 0)
                        {
                            products = products.OrderBy(p => p.UnitInStock);
                        }
                        else
                        {
                            products = products.OrderByDescending(p => p.UnitInStock);
                        }
                        break;
                }

                // paging
                var pager = new ListPager(products.ToList(), page, AppSettings.DEFAULT_PAGE_SIZE);
                return Json(new
                {
                    Success = true,
                    Message = "Filter products in stock success",
                    RowCount = pager.RowCount,
                    PageIndex = pager.PageIndex,
                    PageSize = pager.PageSize,
                    PageTotal = pager.PageTotal,
                    Products = pager.PagedData
                });
            }
            catch (Exception ex)
            {
                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Stock - FilterProductsInStock()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);

                return Json(new { Success = false, Message = ex.Message });
            }
        }

    }
}