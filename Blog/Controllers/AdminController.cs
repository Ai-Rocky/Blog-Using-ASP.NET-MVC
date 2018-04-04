using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Models;
using Blog.ViewModels;
using System.IO;
using Blog.Auth;

namespace Blog.Controllers
{
    public class AdminController : Controller
    {
        
        public ActionResult Login()
        {
            if (Session["Admin"] != null)
                return RedirectToAction("Dashboard");

            return View();
        }

        [HttpPost]
        public ActionResult Login(tbl_admin tbl_Admin)
        {
            using (BlogEntities db = new BlogEntities())
            {
                var AdminInfo = db.tbl_admin.FirstOrDefault(x => x.Email == tbl_Admin.Email && x.Password == tbl_Admin.Password);

                if (AdminInfo == null)
                    ViewBag.Error = "Wrong Information";
                else
                {
                    Session["Admin"] = AdminInfo;
                    return RedirectToAction("Dashboard");
                }

            }

            return View();
        }

        [AdminAuthCheck]
        public ActionResult Dashboard()
        {
            
            using (BlogEntities db = new BlogEntities())
            {
                var viewModel = new ArticleViewModel()
                {
                    category = db.tbl_category.ToList(),
                    article = db.tbl_article.ToList(),
                    blogger = db.tbl_blogger.ToList()
                };

                return View(viewModel);
            }
            
        }

        [AdminAuthCheck]
        public ActionResult Article()
        {
            using (BlogEntities db = new BlogEntities())
            {
                var article = db.tbl_article.Include(c => c.tbl_category).Include(b => b.tbl_blogger).ToList();
                return View(article);
            };
        }

        [HttpPost]
        public ActionResult ArticleUpdate(int id, tbl_article tbl_Article)
        {
            using (BlogEntities db = new BlogEntities())
            {
                var article = db.tbl_article.FirstOrDefault(x => x.ID == id);
                article.Status = tbl_Article.Status;
                db.SaveChanges();
            }
            return RedirectToAction("Article");
        }

        [HttpPost]
        public ActionResult ArticleDelete(int id, tbl_article tbl_Article)
        {
            using (BlogEntities db = new BlogEntities())
            {
                db.Entry(tbl_Article).State = EntityState.Deleted;
                db.SaveChanges();
            }
            return RedirectToAction("Article");
        }

        [AdminAuthCheck]
        public ActionResult Blogger()
        {
            using (BlogEntities db = new BlogEntities())
            {
                return View(db.tbl_blogger.Include(a => a.tbl_article).ToList());
            }
        }

        [HttpPost]
        public ActionResult BloggerDelete(int id, tbl_blogger tbl_Blogger)
        {
            using (BlogEntities db = new BlogEntities())
            {
                db.Entry(tbl_Blogger).State = EntityState.Deleted;
                var article = db.tbl_article.Where(x => x.BloggerID == id);
                db.tbl_article.RemoveRange(article);
                db.SaveChanges();
            }
            return RedirectToAction("Blogger");
        }

        [AdminAuthCheck]
        public ActionResult Category()
        {
            using (BlogEntities db = new BlogEntities())
            {
                return View(db.tbl_category.Include(a=> a.tbl_article).ToList());
            }
        }

        [HttpPost]
        public ActionResult CategoryCreate(tbl_category tbl_Category)
        {
            using (BlogEntities db = new BlogEntities())
            {
                db.tbl_category.Add(tbl_Category);
                db.SaveChanges();
            }
            return RedirectToAction("Category");
        }

        [HttpPost]
        public ActionResult CategoryUpdate(int id, tbl_category tbl_Category)
        {
            using (BlogEntities db = new BlogEntities())
            {
                db.Entry(tbl_Category).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Category");
        }

        [HttpPost]
        public ActionResult CategoryDelete(int id, tbl_category tbl_Category)
        {
            using (BlogEntities db = new BlogEntities())
            {
                db.Entry(tbl_Category).State = EntityState.Deleted;
                var article = db.tbl_article.Where(x => x.CategoryID == id);
                db.tbl_article.RemoveRange(article);
                db.SaveChanges();
            }
            return RedirectToAction("Category");
        }

        [AdminAuthCheck]
        public ActionResult Setting()
        {
            return View();
        }

        [AdminAuthCheck]
        public ActionResult AdminView()
        {
            using (BlogEntities db = new BlogEntities())
            {
                return View(db.tbl_admin.ToList());
            }
        }

        [HttpPost]
        public ActionResult AdminCreate(tbl_admin tbl_Admin)
        {
            using (BlogEntities db = new BlogEntities())
            {
                db.tbl_admin.Add(tbl_Admin);
                db.SaveChanges();
            }
            return RedirectToAction("AdminView");
        }

        [HttpPost]
        public ActionResult AdminUpdate(int id, string btn ,tbl_admin tbl_Admin)
        {
            using (BlogEntities db = new BlogEntities())
            {
                var admin = db.tbl_admin.FirstOrDefault(x => x.ID == id);
                var session = (tbl_admin)Session["Admin"];

                if (btn == "name")
                {
                    admin.Name = tbl_Admin.Name;
                    session.Name = admin.Name;
                }

                else if (btn == "email")
                {
                    admin.Email = tbl_Admin.Email;
                    session.Email = admin.Email;
                }

                else
                {
                    if (admin.Password == Request["OldPassword"])
                    {
                        admin.Password = Request["NewPassword"];
                        db.SaveChanges();

                        return RedirectToAction("Logout");
                    }
                    else
                    {
                        TempData["PassError"] = "Current Password Not Match..";
                        return RedirectToAction("Setting");
                    }
                }

                db.SaveChanges();
            }
            return RedirectToAction("Setting");
        }

        [HttpPost]
        public ActionResult AdminDelete(int id, tbl_admin tbl_Admin)
        {
            using (BlogEntities db = new BlogEntities())
            {
                db.Entry(tbl_Admin).State = EntityState.Deleted;
                db.SaveChanges();
            }
            return RedirectToAction("AdminView");
        }

        public ActionResult Logout()
        {
            Session.Remove("Admin");
            return RedirectToAction("Login");
        }
    }
}