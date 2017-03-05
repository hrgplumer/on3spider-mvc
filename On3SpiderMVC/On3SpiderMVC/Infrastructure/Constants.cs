using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace On3SpiderMVC.Infrastructure
{
    public class Constants
    {
        public static string UploadFileLocation => ConfigurationManager.AppSettings["UploadFileLocation"];

        public static IEnumerable<string> FileCategories => new List<string> {
            //String.Empty,
            "Roster",
            //"Schedule",
            //"Homepage"
        };
    }
}