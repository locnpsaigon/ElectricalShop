using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using ElectricalShop.Common;

namespace ElectricalShop.Models.DataAccess
{
    public class User
    {
        public int UserId { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }
        public String Salt { get; set; }
        public String FullName { get; set; }
        public String Phone { get; set; }
        public String Email { get; set; }
        public Boolean IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
    }

    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        [Required]
        public String RoleName { get; set; }
        public String Description { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }

    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public String CategoryName { get; set; }
        public String Description { get; set; }
        public int? ParrentId { get; set; }
        public int SortIndex { get; set; }
    }

    public class CategoryNode
    {
        public int CategoryId { get; set; }
        public String CategoryName { get; set; }
        public String Description { get; set; }
        public int? ParrentId { get; set; }
        public int SortIndex { get; set; }
        public int TotalProducts { get; set; }
        public int Level { get; set; }

        public CategoryNode()
        {
            TotalProducts = 0;
            Level = 0;
        }
    }

    public class CategoryHierarchy
    {
        public static IList<CategoryNode> AppendChildCategories(ref IList<CategoryNode> listOfNodes, CategoryNode parrent = null)
        {
            using (DBContext db = new DBContext())
            {
                int parentLevel = (parrent == null) ? 0 : parrent.Level;
                int? parentId = (parrent == null) ? null : parrent.CategoryId as int?;

                // Get nodes 
                var childNodes = db.Categories
                    .Where(c => c.ParrentId == parentId)
                    .OrderBy(c => c.SortIndex)
                    .Select(c => new CategoryNode
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName,
                        Description = c.Description,
                        ParrentId = c.ParrentId,
                        TotalProducts = 0,
                        Level = parentLevel + 1
                    })
                    .ToList();

                // Append nodes to list
                if (listOfNodes == null) listOfNodes = new List<CategoryNode>();

                foreach (var node in childNodes)
                {
                    listOfNodes.Add(node);
                    AppendChildCategories(ref listOfNodes, node);
                }
            }
            return listOfNodes;
        }

        public static int CountCategoryProducts(int categoryId)
        {
            using (var db = new DBContext())
            {
                var category = db.Categories.Where(c => c.CategoryId == categoryId).FirstOrDefault();
                if (category != null)
                {
                    /*
                    var count = db.Products.Count(p => p.CategoryId == category.CategoryId);
                    var childCategories = db.Categories.Where(p => p.ParrentId == category.CategoryId).ToList();
                    foreach(var child in childCategories)
                    {
                        count += CountCategoryProducts(child.CategoryId);
                    }
                    */
                    var count = 0;
                    return count;
                }

                return 0;
            }
        }
    }

    public class Brand
    {
        [Key]
        public int BrandId { get; set; }
        public String BrandName { get; set; }
        public String Description { get; set; }
    }

    public class Supplier
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreationUser { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string LastUpdateUser { get; set; }
    }

    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductNameUnsign { get; set; }
        public string Description { get; set; }
        public string DescriptionUnsign { get; set; }
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public string SKU { get; set; }
        public string QuantityPerUnit { get; set; }
        public int UnitInStock { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreationUser { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string LastUpdateUser { get; set; }
        public int Status { get; set; }
    }

    public class Import
    {
        public int ImportId { get; set; }
        public DateTime ImportDate { get; set; }
        public int SupplierId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal ImportPrice { get; set; }
        public string Note { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreationUser { get; set; }
    }

    public class Export
    {
        public int ExportId { get; set; }
        public DateTime ExportDate { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal ExportPrice { get; set; }
        public string Note { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreationUser { get; set; }
    }

    public class Log
    {
        public int LogId { get; set; }
        public DateTime LogDate { get; set; }
        public String Action { get; set; }
        public String Tags { get; set; }
        public String Message { get; set; }
    }

    public class DropDownListOption
    {
        public static List<SelectListItem> GetBrandOptions(int selectedValue = 0)
        {
            using (var db = new DBContext())
            {
                return db.Brands.OrderBy(b => b.BrandName).Select(x => new SelectListItem
                {
                    Text = x.BrandName,
                    Value = x.BrandId.ToString(),
                    Selected = (x.BrandId == selectedValue)
                }).ToList();
            }
        }

        public static List<SelectListItem> GetCategoryOptions(int selectedValue = 0)
        {
            IList<CategoryNode> categories = new List<CategoryNode>();
            CategoryHierarchy.AppendChildCategories(ref categories, null);
            return categories.Select(x => new SelectListItem
            {
                Text = StringExtensions.Repeat("\xA0", x.Level * 4) + x.CategoryName,
                Value = x.CategoryId.ToString(),
                Selected = (x.CategoryId == selectedValue)
            }).ToList();
        }

        public static List<SelectListItem> GetProductOptions(int selectedValue = 0)
        {
            using (var db = new DBContext())
            {
                return (from t1 in db.Products
                        join t2 in db.Brands on t1.BrandId equals t2.BrandId into j1
                        from t3 in j1.DefaultIfEmpty()
                        orderby t3.BrandName ascending, t1.ProductName ascending
                        select new SelectListItem
                        {
                            Text = t3.BrandName + " - " + t1.ProductName,
                            Value = t1.ProductId.ToString(),
                            Selected = (t1.ProductId == selectedValue)
                        }).ToList();
            }
        }

        public static List<SelectListItem> GetSupplierOptions(int selectedValue = 0)
        {
            using (var db = new DBContext())
            {
                return (from t1 in db.Suppliers
                        orderby t1.SupplierId ascending
                        select new SelectListItem
                        {
                            Text = t1.SupplierName,
                            Value = t1.SupplierId.ToString(),
                            Selected = (t1.SupplierId == selectedValue)
                        }).ToList();
            }
        }
    }

}