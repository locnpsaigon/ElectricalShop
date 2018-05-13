using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ElectricalShop.Models;
using ElectricalShop.Models.Admin;
using ElectricalShop.Models.DataAccess;
using ElectricalShop.Models.Principle;
using ElectricalShop.Common;

namespace ElectricalShop.Controllers.Admin
{
    public class CategoryController : BaseController
    {
        DBContext db = new DBContext();

        // GET: Category
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult Index()
        {
            // Write action log
            Log log = new Log();
            log.LogDate = DateTime.Now;
            log.Action = "View Roles";
            log.Tags = GetRequestedIP() + "," + GetLogonUserName();
            log.Message = "Xem danh mục loại hàng";
            LogWritter.WriteLog(log);

            return View();
        }

        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult Add(int? id = 0)
        {
            var model = new AddCategory();

            // Get category tree
            IList<CategoryNode> nodes = new List<CategoryNode>();
            CategoryHierarchy.AppendChildCategories(ref nodes, null);

            // Generate select list items
            model.SelectCategoryItems = nodes.Select(x => new SelectListItem
            {
                Text = StringExtensions.Repeat("\xA0", x.Level * 4) + x.CategoryName,
                Value = x.CategoryId.ToString(),
                Selected = (x.CategoryId == id)
            }).ToList();

            // Add top root category
            model.SelectCategoryItems.Insert(0, new SelectListItem
            {
                Text = "-- Chọn loại hàng cha--",
                Value = "0",
                Selected = (id == null || id == 0)
            });

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult Add(AddCategory model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.ParrentId == 0)
                    {
                        // Add root category
                        var category = new Category();
                        category.CategoryName = model.CategoryName;
                        category.Description = model.Description;
                        category.ParrentId = null;
                        category.SortIndex = db.Categories.DefaultIfEmpty().Max(c => c == null ? 0 : c.SortIndex) + 1;
                        db.Categories.Add(category);
                        db.SaveChanges();

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        // Get parrent category
                        var parrent = db.Categories.Where(c => c.CategoryId == model.ParrentId).FirstOrDefault();
                        if (parrent != null)
                        {
                            // Was category name existed?
                            var wasCategoryNameExisted = db.Categories.Count(c =>
                                c.ParrentId == model.ParrentId &&
                                String.Compare(c.CategoryName, model.CategoryName, true) == 0) > 0;
                            if (!wasCategoryNameExisted)
                            {
                                var category = new Category();
                                category.CategoryName = model.CategoryName;
                                category.Description = model.Description;
                                category.ParrentId = model.ParrentId;
                                category.SortIndex = db.Categories.Count() + 1;
                                db.Categories.Add(category);
                                db.SaveChanges();

                                return RedirectToAction("Index");
                            }
                            else
                            {
                                ModelState.AddModelError("", "Thêm loại hàng thất bại! Tên loại hàng hóa đã tồn tại.");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Thêm loại hàng thất bại! Không tìm thấy loại hàng gốc (cha).");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Category - Add()";
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
            var model = new EditCategory();

            try
            {
                var category = db.Categories.Where(c => c.CategoryId == id).FirstOrDefault();
                if (category != null)
                {
                    model.CategoryId = category.CategoryId;
                    model.CategoryName = category.CategoryName;
                    model.Description = category.Description;
                    model.ParrentId = category.ParrentId;

                    // Get category tree
                    IList<CategoryNode> nodes = new List<CategoryNode>();
                    CategoryHierarchy.AppendChildCategories(ref nodes, null);

                    // Generate select list items
                    model.SelectCategoryItems = nodes.Where(c => c.CategoryId != model.CategoryId).Select(x => new SelectListItem
                    {
                        Text = StringExtensions.Repeat("\xA0", x.Level * 4) + x.CategoryName,
                        Value = x.CategoryId.ToString(),
                        Selected = (x.CategoryId == model.ParrentId)
                    }).ToList();

                    // Add top root category
                    model.SelectCategoryItems.Insert(0, new SelectListItem
                    {
                        Text = "-- Chọn loại hàng cha--",
                        Value = "0",
                        Selected = (category.ParrentId == null)
                    });

                    return View(model);
                }
                else
                {
                    ModelState.AddModelError("", "Không tìm thấy danh mục chỉnh sửa.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Category - Edit()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult Edit(EditCategory model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var category = db.Categories.Where(c => c.CategoryId == model.CategoryId).FirstOrDefault();
                    if (category != null)
                    {
                        if (model.ParrentId == 0)
                        {
                            // Change category to top first category
                            category.CategoryName = model.CategoryName;
                            category.Description = model.Description;
                            category.ParrentId = null;
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            // Get parrent category
                            var parrent = db.Categories.Where(c => c.CategoryId == model.ParrentId).FirstOrDefault();
                            if (parrent != null)
                            {
                                category.CategoryName = model.CategoryName;
                                category.Description = model.Description;
                                category.ParrentId = model.ParrentId;
                                db.SaveChanges();
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                ModelState.AddModelError("", "Cập nhật loại hàng thất bại! Không tìm thấy loại hàng gốc (cha).");
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Cập nhật loại thất bại! Không tìm thấy loại hàng cần chỉnh sửa.");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Category - Edit()";
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
                var category = db.Categories.Where(c => c.CategoryId == id).FirstOrDefault();
                if (category != null)
                {
                    // Category have any child catgeories?
                    var hasChildCategories = db.Categories.Count(c => c.ParrentId == category.CategoryId) > 0;
                    if (hasChildCategories == false)
                    {
                        db.Categories.Remove(category);
                        db.SaveChanges();

                        return Json(new
                        {
                            Success = true,
                            Message = "Xóa loại hàng thành công!"
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Xóa loại hàng thất bại! Loại hàng cần xóa phải rỗng và không chứa loại hàng con."
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Xóa loại hàng thất bại! Không tìm thấy loại hàng cần xóa."
                    });
                }
            }
            catch (Exception ex)
            {
                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Category - Delete()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);

                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public JsonResult MoveUp(int id)
        {
            try
            {
                // Get category info
                var category = db.Categories.Where(c => c.CategoryId == id).FirstOrDefault();
                if (category != null)
                {
                    // Get categories which have same parrent category
                    var sameParrentCategories = db.Categories
                        .OrderBy(c => c.SortIndex)
                        .Where(c => c.ParrentId == category.ParrentId)
                        .ToList();

                    // Category already on top?
                    if (sameParrentCategories.Count() > 0 && sameParrentCategories[0].CategoryId == category.CategoryId)
                    {
                        return Json(new { Success = true, Message = "Di chuyển loại hàng thành công!" });
                    }

                    // Move category up
                    for (int i = 0; i < sameParrentCategories.Count(); i++)
                    {
                        if (sameParrentCategories[i].CategoryId == category.CategoryId)
                        {
                            int sortIndex = category.SortIndex;
                            category.SortIndex = sameParrentCategories[i - 1].SortIndex;
                            sameParrentCategories[i - 1].SortIndex = sortIndex;
                            db.SaveChanges();
                            return Json(new { Success = true, Message = "Di chuyển loại hàng thành công!" });
                        }
                    }
                    return Json(new { Success = true, Message = "Di chuyển loại hàng thành công!" });
                }
                else
                {
                    return Json(new { Success = false, Message = "Di chuyển loại hàng thất bại! Không tìm thấy loại hàng cần chuyển." });
                }
            }
            catch (Exception ex)
            {
                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Category - MoveUp()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);

                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public JsonResult MoveDown(int id)
        {
            try
            {
                // Get category info
                var category = db.Categories.Where(c => c.CategoryId == id).FirstOrDefault();
                if (category != null)
                {
                    // Get categories which have same parrent category
                    var sameParrentCategories = db.Categories
                        .OrderBy(c => c.SortIndex)
                        .Where(c => c.ParrentId == category.ParrentId)
                        .ToList();

                    // Was category alread in bottom?
                    if (sameParrentCategories.Count() > 0 &&
                        sameParrentCategories[sameParrentCategories.Count() - 1].CategoryId == category.CategoryId)
                    {
                        return Json(new { Success = true, Message = "Di chuyển loại hàng thành công!" });
                    }

                    // Move down category
                    for (int i = 0; i < sameParrentCategories.Count() - 1; i++)
                    {
                        if (sameParrentCategories[i].CategoryId == category.CategoryId)
                        {
                            int sortIndex = category.SortIndex;
                            category.SortIndex = sameParrentCategories[i + 1].SortIndex;
                            sameParrentCategories[i + 1].SortIndex = sortIndex;
                            db.SaveChanges();
                            return Json(new { Success = true, Message = "Di chuyển loại hàng thành công!" });
                        }
                    }
                    return Json(new { Success = true, Message = "Di chuyển loại hàng thành công!" });
                }
                else
                {
                    return Json(new { Success = false, Message = "Di chuyển loại hàng thất bại! Không tìm thấy loại hàng cần chuyển." });
                }
            }
            catch (Exception ex)
            {
                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Category - MoveDown()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);

                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        //[OutputCache(Duration = 900, VaryByParam = "parrentId")]
        public JsonResult GetCategoryTree(int parrentId = 0)
        {
            try
            {
                IList<CategoryNode> categories = new List<CategoryNode>();
                if (parrentId == 0)
                {
                    // get root category tree
                    CategoryHierarchy.AppendChildCategories(ref categories, null);
                }
                else
                {
                    // get parrent category
                    var parrentNode = db.Categories.Where(c => c.CategoryId == parrentId).Select(x => new CategoryNode
                    {
                        CategoryId = x.CategoryId,
                        CategoryName = x.CategoryName,
                        Description = x.Description,
                        ParrentId = x.ParrentId,
                        SortIndex = x.SortIndex,
                        TotalProducts = 0,
                        Level = 0
                    })
                    .FirstOrDefault();

                    // append child categories
                    if (parrentNode != null)
                    {
                        categories.Add(parrentNode);
                        CategoryHierarchy.AppendChildCategories(ref categories, parrentNode);
                    }
                }

                // calculate category products
                foreach(var node in categories)
                {
                    node.TotalProducts = CategoryHierarchy.CountCategoryProducts(node.CategoryId);
                }

                return Json(new
                {
                    Success = true,
                    Message = "Get category tree success",
                    Categories = categories
                });
            }
            catch (Exception ex)
            {
                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Category - GetCategoryTree()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);

                return Json(new { Success = false, Message = ex.Message });
            }
        }
    }
}