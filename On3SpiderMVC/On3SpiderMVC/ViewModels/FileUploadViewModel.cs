using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace On3SpiderMVC.ViewModels
{
    public class FileUploadViewModel
    {
        public IEnumerable<SelectListItem> CategoryDropDown { get; set; }
        public string CategoryDropDownSelection { get; set; }
    }
}