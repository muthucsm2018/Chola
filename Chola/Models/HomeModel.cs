using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chola.Models
{
    public class HomeModel
    {

    }

    public class ParentMenu
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public string PageUrl { get; set; }
        public IList<ChildMenu> childs { get; set; }
    }

    public class ChildMenu
    {
        public int PageID { get; set; }
        public string PageTitle { get; set; }
        public string NavigateUrl { get; set; }
    }   
   
}