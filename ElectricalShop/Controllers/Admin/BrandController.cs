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
    public class BrandController : BaseController
    {
        DBContext db = new DBContext();

        // GET: Brand
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult Index()
        {
            // Write action log
            Log log = new Log();
            log.LogDate = DateTime.Now;
            log.Action = "View Roles";
            log.Tags = GetRequestedIP() + "," + GetLogonUserName();
            log.Message = "Xem danh mục thương hiệu";
            LogWritter.WriteLog(log);

            return View();
        }

        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult Add()
        {
            return View();
        }

        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        [HttpPost]
        public ActionResult Add(AddBrand model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Was brand name existed?
                    var wasBrandExisted = db.Brands.Count(b => String.Compare(b.BrandName, model.BrandName) == 0) > 0;
                    if (wasBrandExisted == false)
                    {
                        var brand = new Brand();
                        brand.BrandName = model.BrandName;
                        brand.Description = model.Description;
                        db.Brands.Add(brand);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Lưu thương hiệu thất bại! Tên thương hiệu đã tồn tại trong hệ thống.");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Brand - Add()";
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
            var model = new EditBrand();
            try
            {
                var brand = db.Brands.Where(b => b.BrandId == id).FirstOrDefault();
                if (brand != null)
                {
                    model.BrandId = brand.BrandId;
                    model.BrandName = brand.BrandName;
                    model.Description = brand.Description;
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError("", "Không tìm thấy nhãn hiệu cần cập nhật!");
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Brand - Edit()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult Edit(EditBrand model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Was brand name existed
                    var wasBrandExisted = db.Brands.Count(b => 
                        String.Compare(b.BrandName, model.BrandName, true) == 0 && b.BrandId != model.BrandId) > 0;
                    if (wasBrandExisted == false)
                    {
                        var brand = db.Brands.Where(b => b.BrandId == model.BrandId).FirstOrDefault();
                        if (brand != null)
                        {
                            brand.BrandName = model.BrandName;
                            brand.Description = model.Description;
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Lưu thương hiệu thất bại! Không tìm thấy thương hiệu cần cập nhật.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Lưu thương hiệu thất bại! Tên thương hiệu đã tồn tại trong hệ thống.");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Brand - Edit()";
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
                var brand = db.Brands.Where(b => b.BrandId == id).FirstOrDefault();
                if (brand != null)
                {
                    db.Brands.Remove(brand);
                    db.SaveChanges();
                }
                return Json(new { Success = true, Message = "Xóa thương hiệu thành công!" });
            }
            catch (Exception ex)
            {
                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Brand - Delete()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);

                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public JsonResult FilterBrands(String filterText = "", int page = 1)
        {
            try
            {
                // Filter brands
                var brands = db.Brands.OrderBy(r => r.BrandName).AsEnumerable();

                if (!string.IsNullOrWhiteSpace(filterText))
                {
                    brands = brands.Where(r => r.BrandName.Contains(filterText.Trim()));
                }
                // Paging brands
                var pager = new ListPager(brands, page, AppSettings.DEFAULT_PAGE_SIZE);
                return Json(new
                {
                    Success = true,
                    Message = "Filter brands success",
                    RowCount = pager.RowCount,
                    PageIndex = pager.PageIndex,
                    PageSize = pager.PageSize,
                    PageTotal = pager.PageTotal,
                    Brands = pager.PagedData
                });
            }
            catch (Exception ex)
            {
                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Brand - FilterBrands()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);

                return Json(new { Success = false, Message = ex.Message });
            }
        }
    }
}