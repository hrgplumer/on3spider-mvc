using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Spider.Interface;

namespace On3SpiderMVC.ViewModels.Home
{
    public class ResultsViewModel
    {
        public IList<ISheetRow> ResultsList { get; set; } 
        public string Error { get; set; }
    }
}