using On3SpiderMVC.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace On3SpiderMVC.Repository
{
    public class CategoryRepository
    {
        public IEnumerable<string> GetFileCategories()
        {
            return Constants.FileCategories;
        }
    }
}