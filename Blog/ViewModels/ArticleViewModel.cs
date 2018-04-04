using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Blog.Models;

namespace Blog.ViewModels
{
    public class ArticleViewModel
    {
        public List<tbl_article> article { get; set; }
        public List<tbl_article> oldArticle { get; set; }
        public List<tbl_article> trendArticle { get; set; }
        public tbl_article details { get; set; }
        public List<tbl_article> related { get; set; }
        public List<tbl_category> category { get; set; }
        public tbl_category singleCategory { get; set; }
        public List<tbl_blogger> blogger { get; set; }
        public tbl_blogger profile { get; set; }
    }
}