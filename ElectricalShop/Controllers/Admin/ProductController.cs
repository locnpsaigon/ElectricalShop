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
    public class ProductController : BaseController
    {
        DBContext db = new DBContext();

        // GET: Product
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
        public ActionResult Add()
        {
            var model = new AddProduct();
            try
            {
                model.Price = "0";
                model.Discount = "0";
                model.BrandSelectOptions = DropDownListOption.GetBrandOptions();
                model.CategorySelectOptions = DropDownListOption.GetCategoryOptions();
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Product - Add()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult Add(AddProduct model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    decimal price = 0;
                    decimal discount = 0;
                    if (Decimal.TryParse(model.Price.Replace(",", ""), out price)  && 
                        Decimal.TryParse(model.Discount.Replace(",", ""), out discount))
                    {
                        var product = new Product();
                        product.ProductName = model.ProductName;
                        product.ProductNameUnsign = StringExtensions.convertToUnSign(model.ProductName).ToLower();
                        product.Description = model.Description;
                        product.DescriptionUnsign = StringExtensions.convertToUnSign(model.Description).ToLower();
                        product.BrandId = model.BrandId;
                        product.CategoryId = model.CategoryId;
                        product.SKU = model.SKU;
                        product.QuantityPerUnit = model.QuantityPerUnit;
                        product.Price = price;
                        product.Discount = discount;
                        product.CreationDate = DateTime.Now;
                        product.CreationUser = GetLogonUserName();
                        product.LastUpdate = null;
                        product.LastUpdateUser = null;

                        db.Products.Add(product);
                        db.SaveChanges();

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Giá hoặc giảm giá sản phẩm không hợp lệ!");
                    }
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Product - Add()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);
            }
            finally
            {
                model.BrandSelectOptions = DropDownListOption.GetBrandOptions(model.BrandId ?? 0);
                model.CategorySelectOptions = DropDownListOption.GetCategoryOptions(model.CategoryId ?? 0);
            }
            
            return View(model);
        }

        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult Edit(int id)
        {
            var model = new EditProduct();
            try
            {
                var product = db.Products.Where(p => p.ProductId == id).FirstOrDefault();
                if (product != null)
                {
                    model.ProductId = product.ProductId;
                    model.ProductName = product.ProductName;
                    model.Description = product.Description;
                    model.BrandId = product.BrandId;
                    model.CategoryId = product.CategoryId;
                    model.SKU = product.SKU;
                    model.QuantityPerUnit = product.QuantityPerUnit;
                    model.Price = product.Price.ToString("#,##0");
                    model.Discount = product.Discount.ToString("#,##0");
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError("", "Không tìm thấy sản phẩm!!!");
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Product - Edit()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);
            }
            finally
            {
                model.BrandSelectOptions = DropDownListOption.GetBrandOptions(model.BrandId ?? 0);
                model.CategorySelectOptions = DropDownListOption.GetCategoryOptions(model.CategoryId ?? 0);
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public ActionResult Edit(EditProduct model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var product = db.Products.Where(p => p.ProductId == model.ProductId).FirstOrDefault();
                    if (product != null)
                    {
                        decimal price = 0;
                        decimal discount = 0;
                        if (Decimal.TryParse(model.Price.Replace(",", ""), out price) &&
                            Decimal.TryParse(model.Discount.Replace(",", ""), out discount))
                        {
                            product.ProductName = model.ProductName;
                            product.ProductNameUnsign = StringExtensions.convertToUnSign(model.ProductName).ToLower();
                            product.Description = model.Description;
                            product.DescriptionUnsign = StringExtensions.convertToUnSign(model.Description).ToLower();
                            product.BrandId = model.BrandId;
                            product.CategoryId = model.CategoryId;
                            product.SKU = model.SKU;
                            product.QuantityPerUnit = model.QuantityPerUnit;
                            product.Price = price;
                            product.Discount = discount;
                            product.LastUpdate = DateTime.Now;
                            product.LastUpdateUser = GetLogonUserName();

                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Giá sản phẩm không hợp lệ!");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Không tìm thấy sản phẩm!!!");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Product - Edit()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);
            }
            finally
            {
                model.BrandSelectOptions = DropDownListOption.GetBrandOptions(model.BrandId ?? 0);
                model.CategorySelectOptions = DropDownListOption.GetCategoryOptions(model.CategoryId ?? 0);
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
                var products = db.Products.Where(p => p.ProductId == id).ToList();
                db.Products.RemoveRange(products);
                db.SaveChanges();

                return Json(new { Success = true, Message = "Xóa sản phẩm thành công!!!" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public JsonResult FilterProducts(string searchText = "", int cat = 0, int brand = 0, int page = 1)
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

                // sort
                productQuery = productQuery.OrderBy(p => p.ProductName);

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

                // paging
                var pager = new ListPager(products.ToList(), page, AppSettings.DEFAULT_PAGE_SIZE);
                return Json(new
                {
                    Success = true,
                    Message = "Filter products success",
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
                log.Action = "Product - FilterProducts()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);

                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        [ElectricalShopAuthorize(Roles = "Administrators")]
        public JsonResult GetAllProducts()
        {
            try
            {
                var productQuery = from p in db.Products select p;
                var products = from p in productQuery
                               join t1 in db.Brands on p.BrandId equals t1.BrandId into j1
                               join t2 in db.Categories on p.CategoryId equals t2.CategoryId into j2
                               from t3 in j1.DefaultIfEmpty()
                               from t4 in j2.DefaultIfEmpty()
                               orderby t3.BrandName, p.ProductName
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
                return Json(new
                {
                    Success = true,
                    Message = "Filter article success",
                    Products = products.ToList()
                });
            }
            catch(Exception ex)
            {
                // Write error log
                var log = new Log();
                log.LogDate = DateTime.Now;
                log.Action = "Product - GetAllProducts()";
                log.Tags = "Error";
                log.Message = ex.ToString();
                db.Logs.Add(log);

                return Json(new { Success = false, Message = ex.Message });
            }
        }

    }
}