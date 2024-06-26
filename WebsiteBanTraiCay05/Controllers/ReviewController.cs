﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanTraiCay05.Models;
using WebsiteBanTraiCay05.Models.EF;

namespace WebsiteBanTraiCay05.Controllers
{
    public class ReviewController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Review
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _Review(int productId)
        {
            ViewBag.ProductId = productId;
            var item = new Review();
            if (User.Identity.IsAuthenticated)
            {
                var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
                var userMangager = new UserManager<ApplicationUser>(userStore);
                var user = userMangager.FindByName(User.Identity.Name);
                if (user != null)
                {
                    item.Email = user.Email;
                    item.FullName = user.FullName;
                    item.UserName = user.UserName;
                }
                return PartialView(item);
            }
            return PartialView();
        }

        public ActionResult _Load_Review(int productId)
        {
            var item = db.Reviews.Where(x => x.ProductId == productId).OrderByDescending(x => x.Id).ToList();
            ViewBag.Count = item.Count;
            return PartialView(item);
        }

        [HttpPost]
        public ActionResult PostReview(Review req)
        {
            if (ModelState.IsValid)
            {
                req.CreateDate = DateTime.Now;
                db.Reviews.Add(req);
                db.SaveChanges();
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }
    }
}