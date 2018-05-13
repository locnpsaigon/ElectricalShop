using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ElectricalShop.Models.Admin;
using ElectricalShop.Common;
using ElectricalShop.Models.DataAccess;
using ElectricalShop.Models.Principle;

namespace ElectricalShop.Controllers.Admin
{
    public class RoleController : BaseController
    {
        DBContext db = new DBContext();

        // GET: Role
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult Index()
        {
            // Write action log
            Log log = new Log();
            log.LogDate = DateTime.Now;
            log.Action = "View Roles";
            log.Tags = GetRequestedIP() + "," + GetLogonUserName();
            log.Message = "Xem danh sách chức danh";
            LogWritter.WriteLog(log);

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
        public ActionResult Add(AddRole model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using(DBContext db = new DBContext())
                    {
                        // role name already existed?
                        var role = db.Roles.Where(r => string.Compare(r.RoleName, model.RoleName) == 0).FirstOrDefault();
                        if (role == null)
                        {   
                            // role not existed, create new role
                            role = new Role();
                            role.RoleName = model.RoleName;
                            role.Description = model.Description;

                            db.Roles.Add(role);
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        } else
                        {
                            ModelState.AddModelError("", "");
                            return View(model);
                        }
                    }
                }
            } catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Role - Add()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);
                db.SaveChanges();
            }
            return View(model);
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            try
            {
                var role = db.Roles.Where(r => r.RoleId == id).FirstOrDefault();
                if (role != null)
                {
                    EditRole model = new EditRole();
                    model.RoleId = role.RoleId;
                    model.RoleName = role.RoleName;
                    model.Description = role.Description;
                    return View(model);
                } else
                {
                    ModelState.AddModelError("", "Không tìm thấy chức danh phù hợp!");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Role - Edit()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);
                db.SaveChanges();
            }
            return View();
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult Edit(EditRole model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Was role name existed
                    var role = db.Roles.Where(r => r.RoleId != model.RoleId && r.RoleName.CompareTo(model.RoleName) == 0).FirstOrDefault();
                    if (role == null)
                    {
                        // Get edited role
                        role = db.Roles.Where(r => r.RoleId == model.RoleId).FirstOrDefault();
                        role.RoleName = model.RoleName;
                        role.Description = model.Description;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    } else
                    {
                        ModelState.AddModelError("", "Tên chức danh [" + model.RoleName + "] đã tồn tại! Bạn vui lòng nhập tên chức danh khác.");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Role - Edit()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);
                db.SaveChanges();
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public JsonResult FilterRoles(String filterText = "", int page = 1)
        {
            try
            {
                // Filter roles
                var roles = db.Roles.OrderBy(r => r.RoleName).Select(x => new
                {
                    RoleId = x.RoleId,
                    RoleName = x.RoleName,
                    Description = x.Description
                });

                if (string.IsNullOrWhiteSpace(filterText) == false)
                {
                    roles = roles.Where(r => r.RoleName.Contains(filterText.Trim()));
                }

                // Paging roles
                var pager = new ListPager(roles.ToList(), page, AppSettings.DEFAULT_PAGE_SIZE);
                return Json(new
                {
                    Success = true,
                    Message = "Filter roles success",
                    RowCount = pager.RowCount,
                    PageIndex = pager.PageIndex,
                    PageSize = pager.PageSize,
                    PageTotal = pager.PageTotal,
                    Roles = pager.PagedData
                });
            }
            catch(Exception ex)
            {
                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Role - FilterRoles()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);
                db.SaveChanges();

                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public JsonResult Delete(int id)
        {
            try
            {
                // Get role info
                var role = db.Roles.Where(r => r.RoleId == id).FirstOrDefault();
                if (role != null)
                {
                    // Remove users from role
                    role.Users.Clear();
                    // Remove role
                    db.Roles.Remove(role);
                    db.SaveChanges();
                }
                return Json(new { Success = true, Message = "Xóa chức danh thành công" });
            }
            catch(Exception ex)
            {
                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Role - Delete()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);
                db.SaveChanges();

                return Json(new { Success = false, Message = ex.Message });
            }
        }
    }
}