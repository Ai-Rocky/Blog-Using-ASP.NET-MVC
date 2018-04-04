using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Models;
using Blog.ViewModels;
using System.Data.Entity;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Search( string Search)
        {
            using (BlogEntities db = new BlogEntities())
            {
                var viewModel = new ArticleViewModel()
                {
                    article = db.tbl_article.Include(c => c.tbl_category).Include(b => b.tbl_blogger).Where(s => s.Status == 1 && s.Title.Contains(Search) || s.Details.Contains(Search) || s.tbl_blogger.Name.Contains(Search) || s.tbl_category.Name.Contains(Search)).OrderByDescending(a => a.ID).ToList(),
                    oldArticle = db.tbl_article.Include(c => c.tbl_category).Include(b => b.tbl_blogger).Where(s => s.Status == 1 && s.Title.Contains(Search) || s.Details.Contains(Search) || s.tbl_blogger.Name.Contains(Search) || s.tbl_category.Name.Contains(Search)).OrderBy(a => a.ID).ToList(),
                    trendArticle = db.tbl_article.Include(c => c.tbl_category).Include(b => b.tbl_blogger).Where(s => s.Status == 1 && s.Title.Contains(Search) || s.Details.Contains(Search) || s.tbl_blogger.Name.Contains(Search) || s.tbl_category.Name.Contains(Search)).OrderByDescending(a => a.Views).ToList()
                };

                return View(viewModel);
            }
        }

        public ActionResult Index()
        {
            using (BlogEntities db = new BlogEntities())
            {
                var viewModel = new ArticleViewModel()
                {
                    article = db.tbl_article.Include(c => c.tbl_category).Include(b => b.tbl_blogger).Where(s=> s.Status == 1).OrderByDescending(a=> a.ID).ToList(),
                    oldArticle = db.tbl_article.Include(c => c.tbl_category).Include(b => b.tbl_blogger).Where(s => s.Status == 1).OrderBy(a => a.ID).ToList(),
                    trendArticle = db.tbl_article.Include(c => c.tbl_category).Include(b => b.tbl_blogger).Where(s => s.Status == 1).OrderByDescending(a => a.Views).ToList(),
                    category = db.tbl_category.Include(a => a.tbl_article).OrderByDescending(c=> c.tbl_article.Count).ToList(),
                    blogger = db.tbl_blogger.Include(a => a.tbl_article).OrderByDescending(b=> b.tbl_article.Count).ToList()
                };

                return View(viewModel);
            }
        }

        public ActionResult Details(int id)
        {
            using (BlogEntities db = new BlogEntities())
            {
                var single = db.tbl_article.Include(c => c.tbl_category).Include(b => b.tbl_blogger).SingleOrDefault(a => a.ID == id);
                if (single.Views == null)
                    single.Views = 1;
                else
                    single.Views = single.Views + 1;

                db.SaveChanges();

                var viewModel = new ArticleViewModel()
                {
                    details = single,
                    related = db.tbl_article.Include(b => b.tbl_blogger).Where(a => a.CategoryID == single.CategoryID && a.ID != single.ID).ToList()
                };

                return View(viewModel);
            }
        }

        public ActionResult Category(int id)
        {
            using (BlogEntities db = new BlogEntities())
            {
                var viewModel = new ArticleViewModel()
                {
                    article = db.tbl_article.Include(b => b.tbl_blogger).Where(a => a.CategoryID == id && a.Status == 1).OrderByDescending(a => a.ID).ToList(),
                    oldArticle = db.tbl_article.Include(b => b.tbl_blogger).Where(a => a.CategoryID == id && a.Status == 1).OrderBy(a => a.ID).ToList(),
                    trendArticle = db.tbl_article.Include(b => b.tbl_blogger).Where(a => a.CategoryID == id && a.Status == 1).OrderByDescending(a => a.Views).ToList(),
                    singleCategory = db.tbl_category.SingleOrDefault(c => c.ID == id)
                };

                return View(viewModel);
            }
        }

        public new ActionResult Profile(int id)
        {
            using (BlogEntities db = new BlogEntities())
            {
                var viewModel = new ArticleViewModel()
                {
                    article = db.tbl_article.Include(c => c.tbl_category).Where(a => a.BloggerID == id && a.Status == 1).OrderByDescending(a => a.ID).ToList(),
                    oldArticle = db.tbl_article.Include(c => c.tbl_category).Where(a => a.BloggerID == id && a.Status == 1).OrderBy(a => a.ID).ToList(),
                    trendArticle = db.tbl_article.Include(c => c.tbl_category).Where(a => a.BloggerID == id && a.Status == 1).OrderByDescending(a => a.Views).ToList(),
                    profile = db.tbl_blogger.SingleOrDefault(b => b.ID == id)
                };

                return View(viewModel);
            }
        }
    }
}