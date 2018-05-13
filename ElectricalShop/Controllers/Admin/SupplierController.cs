using System;
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
    public class SupplierController : BaseController
    {
        DBContext db = new DBContext();

        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult Add(AddSupplier model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var supplier = new Supplier();
                    supplier.SupplierName = model.SupplierName;
                    supplier.Description = model.Description;
                    supplier.Address = model.Address;
                    supplier.Phone = model.Phone;
                    supplier.Fax = model.Fax;
                    supplier.Email = model.Email;
                    supplier.Website = model.Website;
                    supplier.CreationUser = GetLogonUserName();
                    supplier.CreationDate = DateTime.Now;
                    supplier.LastUpdate = null;
                    supplier.LastUpdateUser = null;

                    db.Suppliers.Add(supplier);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Supplier - Add()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);
            }
            return View(model);
        }

        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult Edit(int id)
        {
            var model = new EditSupplier();
            try
            {
                var supplier = db.Suppliers.Where(s => s.SupplierId == id).FirstOrDefault();
                if (supplier != null)
                {
                    model.SupplierId = supplier.SupplierId;
                    model.SupplierName = supplier.SupplierName;
                    model.Description = supplier.Description;
                    model.Address = supplier.Address;
                    model.Phone = supplier.Phone;
                    model.Fax = supplier.Fax;
                    model.Email = supplier.Email;
                    model.Website = supplier.Website;
                }
                else
                {
                    ModelState.AddModelError("", "Không tìm thấy nhà cung cấp cần cập nhật!");
                }
            }
            catch(Exception ex) {
                ModelState.AddModelError("", ex.Message);

                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Supplier - Edit()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);
            } 
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult Edit(EditSupplier model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var supplier = db.Suppliers.Where(s => s.SupplierId == model.SupplierId).FirstOrDefault();
                    if (supplier != null)
                    {
                        supplier.SupplierName = model.SupplierName;
                        supplier.Description = model.Description;
                        supplier.Address = model.Address;
                        supplier.Phone = model.Phone;
                        supplier.Fax = model.Fax;
                        supplier.Email = model.Email;
                        supplier.Website = model.Website;
                        supplier.LastUpdate = DateTime.Now;
                        supplier.LastUpdateUser = GetLogonUserName();
                        db.SaveChanges();

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Không tìm thấy nhà cung cấp cần cập nhật!");
                    }
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Supplier - Edit()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public JsonResult Delete(int id)
        {
            try
            {
                // Check supplier existed from import history

                // Delete supplier
                var suppliers = db.Suppliers.Where(s => s.SupplierId == id).ToList();
                db.Suppliers.RemoveRange(suppliers);
                db.SaveChanges();
                return Json(new { Success = true, Message = "Delete supplier success!!!" });
            }
            catch(Exception ex)
            {
                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Supplier - Delete()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);

                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public JsonResult FilterSuppliers(String filterText = "", int page = 1)
        {
            try
            {
                // Filter suppliers
                var suppliers = db.Suppliers.OrderBy(r => r.SupplierName).AsEnumerable();

                if (!string.IsNullOrWhiteSpace(filterText))
                {
                    suppliers = suppliers.Where(s => s.SupplierName.Contains(filterText.Trim()));
                }
                // Paging brands
                var pager = new ListPager(suppliers, page, AppSettings.DEFAULT_PAGE_SIZE);
                return Json(new
                {
                    Success = true,
                    Message = "Filter suppliers success",
                    RowCount = pager.RowCount,
                    PageIndex = pager.PageIndex,
                    PageSize = pager.PageSize,
                    PageTotal = pager.PageTotal,
                    Suppliers = pager.PagedData
                });
            }
            catch (Exception ex)
            {
                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Suppplier - FilterSuppliers()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);

                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public JsonResult GetAllSuppliers()
        {
            try
            {
                var suppliers = db.Suppliers.OrderBy(s => s.SupplierName).ToList();
                return Json(new
                {
                    Success = true,
                    Message = "Get suppliers list success",
                    Suppliers = suppliers
                });
            }
            catch (Exception ex)
            {
                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Supplier - GetAllSuppliers()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);

                return Json(new { Success = false, Message = ex.Message });
            }
        }
    }
}