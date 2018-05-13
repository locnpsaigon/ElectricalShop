using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json;
using ElectricalShop.Models.Admin;
using ElectricalShop.Models.DataAccess;
using ElectricalShop.Models.Principle;
using ElectricalShop.Common;

namespace ElectricalShop.Controllers.Admin
{
    public class AuthController : BaseController
    {
        DBContext db = new DBContext();

        [AllowAnonymous]
        public ActionResult Login()
        {
            Login model = new Login();
            model.UserName = "admin";
            model.Password = "123456aA@";
            model.RememberMe = true;
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(Login model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Where(u => String.Compare(u.UserName, model.UserName) == 0).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsActive)
                    {
                        // Verify user password
                        var success = SaltedHash.Verify(user.Salt, user.Password, model.Password);
                        if (success)
                        {
                            // Save authentication info
                            ElectricalShopPrincipleModel principle = new ElectricalShopPrincipleModel();
                            principle.UserId = user.UserId;
                            principle.FullName = user.FullName;
                            principle.Roles = user.Roles.Select(r => r.RoleName).ToArray();

                            // Add authentication cookie
                            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, model.UserName, 
                                DateTime.Now, DateTime.Now.AddDays(7), model.RememberMe, JsonConvert.SerializeObject(principle));
                            String authTicketEncrypted = FormsAuthentication.Encrypt(authTicket);
                            HttpCookie asCookie = new HttpCookie(FormsAuthentication.FormsCookieName, authTicketEncrypted);
                            Response.Cookies.Add(asCookie);

                            // Write action log
                            Log log = new Log();
                            log.LogDate = DateTime.Now;
                            log.Action = "Login";
                            log.Tags = GetRequestedIP() + "," + model.UserName;
                            log.Message = "Đăng nhập hệ thống";
                            LogWritter.WriteLog(log);

                            return RedirectToAction("Index", "Admin");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Sai mật khẩu!");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Tài khoản đã bị khóa!");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Tài khoản không tồn tại trong hệ thống!");
                }
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult SignOut()
        {
            // Write action log
            Log log = new Log();
            log.LogDate = DateTime.Now;
            log.Action = "SignOut";
            log.Tags = GetRequestedIP() + "," + GetLogonUserName();
            log.Message = "Đăng xuất hệ thống";
            LogWritter.WriteLog(log);

            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [Authorize]
        public ActionResult Profiler()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = db.Users.Where(u => u.UserId == User.UserId).FirstOrDefault();
                if (user != null)
                {
                    var model = new Profiler();
                    model.UserId = user.UserId;
                    model.UserName = user.UserName;
                    model.FullName = user.FullName;
                    model.Phone = user.Phone;
                    model.Email = user.Email;
                    return View(model);
                }
                else
                {
                    return RedirectToAction("SignOut");
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult Profiler(Profiler model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Where(u => u.UserId == model.UserId).FirstOrDefault();
                if (user != null)
                {
                    if (String.IsNullOrWhiteSpace(model.NewPassword))
                    {
                        user.FullName = model.FullName;
                        user.Phone = model.Phone;
                        user.Email = model.Email;
                        db.SaveChanges();
                        RedirectToAction("Index", "Admin");
                    }
                    else
                    {   
                        /* User changed password */
                        if (SaltedHash.Verify(user.Salt, user.Password, model.Password))
                        {
                            SaltedHash sh = new SaltedHash(model.NewPassword);
                            user.Password = sh.Hash;
                            user.Salt = sh.Salt;
                            user.FullName = model.FullName;
                            user.Phone = model.Phone;
                            user.Email = model.Email;
                            db.SaveChanges();

                            // Write action log
                            Log log = new Log();
                            log.LogDate = DateTime.Now;
                            log.Action = "Update profile";
                            log.Tags = GetRequestedIP() + "," + model.UserName;
                            log.Message = "Cập nhật thông tin cá nhân";
                            LogWritter.WriteLog(log);

                            RedirectToAction("Index", "Admin");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Sai mật khẩu!");
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
            return View(model);
        }

    }
}