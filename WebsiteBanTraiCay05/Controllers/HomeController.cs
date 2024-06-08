using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanTraiCay05.Models;
using WebsiteBanTraiCay05.Models.EF;

namespace WebsiteBanTraiCay05.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Partial_Subcrice()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult Subscribe(Subscribe req)
        {
            if (ModelState.IsValid)
            {
                db.Subscribes.Add(new Subscribe { Email = req.Email, CreatedDate = DateTime.Now });
                db.SaveChanges();
                return Json(new { Success = true });
            }
            return View("Partial_Subcrice", req);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Refresh()
        {
            var item = new StatisticalModel();
            ViewBag.Visitors_online = HttpContext.Application["visitors_online"];
            item.Today = HttpContext.Application["Today"].ToString();
            item.Yesterday = HttpContext.Application["Yesterday"].ToString();
            item.ThisWeek = HttpContext.Application["ThisWeek"].ToString();
            item.LastWeek = HttpContext.Application["LastWeek"].ToString();
            item.ThisMonth = HttpContext.Application["ThisMonth"].ToString();
            item.LastMonth = HttpContext.Application["LastMonth"].ToString();
            item.Total = HttpContext.Application["Total"].ToString();
            return PartialView(item);
        }
    }
}