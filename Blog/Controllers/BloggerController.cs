using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Models;
using System.IO;
using System.Dynamic;
using Blog.ViewModels;
using System.Data.Entity;
using Blog.Auth;

namespace Blog.Controllers
{
    public class BloggerController : Controller
    {
        [HttpPost]
        public ActionResult SignUp(tbl_blogger tbl_Blogger)
        {
            using (BlogEntities db = new BlogEntities())
            {
                var BloggerInfo = db.tbl_blogger.FirstOrDefault(x => x.Email == tbl_Blogger.Email);
                if (BloggerInfo == null)
                {
                    db.tbl_blogger.Add(tbl_Blogger);
                    db.SaveChanges();
                    TempData["Success"] = "Your account create successful...";
                }
                else
                    TempData["Error"] = "The email is already registred...";
            }
            return RedirectToAction("Index","Home");
        }

        [HttpPost]
        public ActionResult Login(tbl_blogger tbl_Blogger)
        {
            using (BlogEntities db = new BlogEntities())
            {
                var BloggerInfo = db.tbl_blogger.FirstOrDefault(x => x.Email == tbl_Blogger.Email && x.Password == tbl_Blogger.Password);

                if (BloggerInfo == null)
                    TempData["Error"] = "Wrong email or password...";
                else
                    Session["Blogger"] = BloggerInfo;

            }

            return RedirectToAction("Index", "Home");
        }

        [BloggerAuthCheck]
        public ActionResult Article()
        {
            using (BlogEntities db = new BlogEntities())
            {
                var session = (tbl_blogger)Session["Blogger"];
                var viewModel = new ArticleViewModel()
                {
                    category = db.tbl_category.Where(s=> s.Status == 1).ToList(),
                    article = db.tbl_article.Include(c => c.tbl_category).Include(b => b.tbl_blogger).Where(a=> a.BloggerID == session.ID).ToList()
                };

                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ArticleCreate(tbl_article tbl_Article, HttpPostedFileBase Image)
        {
            using (BlogEntities db = new BlogEntities())
            {
                if (Image != null)
                {
                    string fileName = Path.GetFileName(Image.FileName);
                    string extension = Path.GetExtension(Image.FileName);
                    if(fileName.Length > 30)
                        fileName = "thumb-" + fileName.Substring(0, 30) + extension;
                    else
                        fileName = "thumb-" + fileName + extension;

                    string filePath = Path.Combine(Server.MapPath("~/Images"), fileName);
                    Image.SaveAs(filePath);

                    tbl_Article.Image = fileName;
                    tbl_Article.Status = 1;

                    db.tbl_article.Add(tbl_Article);
                    db.SaveChanges();
                }
                else
                    TempData["Error"] = "Somthing is wrong..";
            }
       
            return RedirectToAction("Article");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ArticleUpdate(int id, tbl_article tbl_Article, HttpPostedFileBase Image, string OldImage)
        {
            using (BlogEntities db = new BlogEntities())
            {
                if (Image != null)
                {
                    string fileName = Path.GetFileName(Image.FileName);
                    string extension = Path.GetExtension(Image.FileName);
                    if (fileName.Length > 30)
                        fileName = "thumb-" + fileName.Substring(0, 30) + extension;
                    else
                        fileName = "thumb-" + fileName + extension;

                    string filePath = Path.Combine(Server.MapPath("~/Images"), fileName);
                    Image.SaveAs(filePath);

                    tbl_Article.Image = fileName;
                }
                else
                    tbl_Article.Image = OldImage;

                db.Entry(tbl_Article).State = EntityState.Modified;
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

        [BloggerAuthCheck]
        public ActionResult Setting()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BloggerUpdate(int id, string btn, tbl_blogger tbl_Blogger, HttpPostedFileBase Image)
        {
            using (BlogEntities db = new BlogEntities())
            {
                var blogger = db.tbl_blogger.FirstOrDefault(x => x.ID == id);
                var session = (tbl_blogger)Session["Blogger"];

                if (btn == "image")
                {
                    if (Image.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(Image.FileName);
                        string filePath = Path.Combine(Server.MapPath("~/Images"), fileName);
                        Image.SaveAs(filePath);

                        blogger.Image = fileName;
                        session.Image = blogger.Image;
                    }
                    else
                        TempData["ArticleError"] = "Somthing is wrong..";
                }

                else if (btn == "name")
                {
                    blogger.Name = tbl_Blogger.Name;
                    session.Name = blogger.Name;
                }

                else if (btn == "email")
                {
                    blogger.Email = tbl_Blogger.Email;
                    session.Email = blogger.Email;
                }

                else if (btn == "password")
                {
                    if (blogger.Password == Request["OldPassword"])
                    {
                        blogger.Password = Request["NewPassword"];
                        db.SaveChanges();

                        return RedirectToAction("Logout");
                    }
                    else
                    {
                        TempData["PassError"] = "Current Password Not Match..";
                        return RedirectToAction("Setting");
                    }
                }

                else if (btn == "intro")
                {
                    blogger.Intro = tbl_Blogger.Intro;
                    session.Intro = blogger.Intro;
                }

                else if (btn == "facebook")
                {
                    blogger.FacebookLink = tbl_Blogger.FacebookLink;
                    session.FacebookLink = blogger.FacebookLink;
                }

                else if (btn == "linkedin")
                {
                    blogger.LinkedinLink = tbl_Blogger.LinkedinLink;
                    session.LinkedinLink = blogger.LinkedinLink;
                }

                else if (btn == "github")
                {
                    blogger.GithubLink = tbl_Blogger.GithubLink;
                    session.GithubLink = blogger.GithubLink;
                }

                db.SaveChanges();
            }
            return RedirectToAction("Setting");
        }

        public ActionResult Logout()
        {
            Session.Remove("Blogger");
            return RedirectToAction("Index", "Home");
        }
    }
}