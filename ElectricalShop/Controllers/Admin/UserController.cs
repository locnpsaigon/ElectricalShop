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
    public class UserController : BaseController
    {
        DBContext db = new DBContext();

        // GET: User
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult Index()
        {
            // Write action log
            Log log = new Log();
            log.LogDate = DateTime.Now;
            log.Action = "View Users";
            log.Tags = GetRequestedIP() + "," + GetLogonUserName();
            log.Message = "Xem danh sách tài khoản";
            LogWritter.WriteLog(log);

            return View();
        }

        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult Add()
        {
            AddUser model = new AddUser();
            try
            {
                var roles = db.Roles.OrderBy(r => r.RoleName).ToList();
                model.RoleCheckBoxes = db.Roles.OrderBy(r => r.RoleName).Select(x => new RoleCheckBox
                {
                    RoleId = x.RoleId,
                    RoleName = x.RoleName,
                    Selected = false
                }).ToList();
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "User - Add()";
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
        public ActionResult Add(AddUser model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Was user name existed?
                    var user = db.Users.Where(u => string.Compare(u.UserName, model.UserName, true) == 0).FirstOrDefault();
                    if (user == null)
                    {
                        // Create user
                        user = new User();
                        user.UserName = model.UserName;
                        var sh = new SaltedHash(model.Password);
                        user.Password = sh.Hash;
                        user.Salt = sh.Salt;
                        user.FullName = model.FullName;
                        user.Phone = model.Phone;
                        user.Email = model.Email;
                        user.IsActive = model.IsActive;
                        user.CreateDate = DateTime.Now;

                        // Add user's role
                        user.Roles = new List<Role>();
                        foreach(var roleCheckBox in model.RoleCheckBoxes)
                        {
                            if (roleCheckBox.Selected)
                            {
                                var role = db.Roles.Where(r => r.RoleId == roleCheckBox.RoleId).FirstOrDefault();
                                if (role != null)
                                {
                                    user.Roles.Add(role);
                                }
                            }
                        }
                        
                        // Save user info
                        db.Users.Add(user);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    } else
                    {
                        ModelState.AddModelError("", "Tài khoản ["  + model.UserName + "] đã tồn tại trong hệ thống! Vui lòng nhập tên đăng nhập khác.");
                    }
                }



            } catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "User - Add()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);
                db.SaveChanges();
            }
            return View(model);
        }

        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult Edit(int id)
        {
            try
            {
                // get edited user
                var user = db.Users.Where(u => u.UserId == id).FirstOrDefault();
                if (user != null)
                {
                    // create view model
                    var model = new EditUser();
                    model.UserId = user.UserId;
                    model.UserName = user.UserName;
                    model.FullName = user.FullName;
                    model.Phone = user.Phone;
                    model.Email = user.Email;
                    model.IsActive = user.IsActive;

                    // create role checkboxe models
                    var roles = db.Roles.OrderBy(r => r.RoleName).ToList();
                    foreach(var role in roles)
                    {
                        var roleCB = new RoleCheckBox();
                        roleCB.RoleId = role.RoleId;
                        roleCB.RoleName = role.RoleName;
                        roleCB.Selected = false;
                        foreach(var userRole in user.Roles)
                        {
                            if (role.RoleId == userRole.RoleId)
                            {
                                roleCB.Selected = true;
                                break;
                            }
                        }
                        model.RoleCheckBoxes.Add(roleCB);
                    }
                    return View(model);
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "User - Edit()";
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
        public ActionResult Edit(EditUser model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // get edited user
                    var user = db.Users.Where(u => u.UserId == model.UserId).FirstOrDefault();
                    if (user != null)
                    {
                        // update password
                        if (!string.IsNullOrWhiteSpace(model.Password))
                        {
                            SaltedHash sh = new SaltedHash(model.Password);
                            user.Password = sh.Hash;
                            user.Salt = sh.Salt;
                        }

                        // update user info
                        user.FullName = model.FullName;
                        user.Phone = model.Phone;
                        user.Email = model.Email;
                        user.IsActive = model.IsActive;
                        // update user's role
                        user.Roles.Clear();
                        foreach(var roleCB in model.RoleCheckBoxes)
                        {
                            if (roleCB.Selected)
                            {
                                var roleSelected = db.Roles.Where(r => r.RoleId == roleCB.RoleId).FirstOrDefault();
                                if (roleSelected != null)
                                {
                                    user.Roles.Add(roleSelected);
                                }
                            }
                        }
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Không tìm thấy thông tin tài khoản!");
                    }
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "User - Edit()";
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
        public JsonResult FilterUsers(string filterText = "", int page = 1)
        {
            try
            {
                // Filter users
                var users = db.Users.OrderBy(r => r.UserName).Select(x => new {
                    UserId = x.UserId,
                    UserName = x.UserName,
                    FullName = x.FullName,
                    Phone = x.Phone,
                    Email = x.Email,
                    IsActive = x.IsActive,
                    CreateDate = x.CreateDate
                }).AsEnumerable();

                if (!string.IsNullOrWhiteSpace(filterText))
                {
                    users = users.Where(u => 
                        u.UserName.Contains(filterText.Trim()) || 
                        u.Phone.Contains(filterText.Trim()) ||
                        u.Email.Contains(filterText.Trim()));
                }
                // Do paging
                var pager = new ListPager(users, page, AppSettings.DEFAULT_PAGE_SIZE);
                return Json(new
                {
                    Success = true,
                    Message = "Filter users success",
                    RowCount = pager.RowCount,
                    PageIndex = pager.PageIndex,
                    PageSize = pager.PageSize,
                    PageTotal = pager.PageTotal,
                    Users = pager.PagedData
                });
            }
            catch (Exception ex)
            {
                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "User - FilterUsers()";
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
                var user = db.Users.Where(u => u.UserId == id).FirstOrDefault();
                if (user != null)
                {
                    // Remove user's roles
                    user.Roles.Clear();
                    db.Users.Remove(user);
                    db.SaveChanges();
                }
                return Json(new { Success = true, Message = "Xóa tài khoản thành công" });
            }
            catch(Exception ex)
            {
                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "User - Delete()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);
                db.SaveChanges();

                return Json(new { Success = false, Message = ex.Message });
            }
        }
    }
}