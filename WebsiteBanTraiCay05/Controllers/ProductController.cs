using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using WebsiteBanTraiCay05.Models;
using WebsiteBanTraiCay05.Models.EF;
using PagedList;

namespace WebsiteBanTraiCay05.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Product
        public ActionResult Index(int? page)
        {
            var pageSize = 9;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<Product> items = db.Products.ToList();
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View(items);
        }

        public ActionResult Detail(string alias, int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                db.Products.Attach(item);
                item.ViewCount = item.ViewCount + 1;
                db.Entry(item).Property(x => x.ViewCount).IsModified = true;
                db.SaveChanges();
            }
            return View(item);
        }

        public ActionResult ProductCategory(string alias, int id)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var items = db.Products.ToList();
                    if (id > 0)
                    {
                        items = items.Where(x => x.ProductCategoryId == id).ToList();
                    }

                    var cate = db.ProductCategories.Find(id);
                    if (cate != null)
                    {
                        ViewBag.CateName = cate.Title;
                    }
                    ViewBag.CateId = id;
                    scope.Complete();
                    return View(items);
                }
                catch 
                {
                    return View("Error");
                }
            }
        }

        public ActionResult Partial_ItemByCateId()
        {
            var products = db.Products.Where(x => x.IsActive).Take(12).ToList();
            return PartialView(products);
        }

        public ActionResult Partial_ItemVegetables()
        {
            var products = db.Products.Where(x => x.IsActive).Take(12).ToList();
            return PartialView(products);
        }
    }
}