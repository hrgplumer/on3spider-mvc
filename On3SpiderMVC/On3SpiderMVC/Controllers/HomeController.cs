using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using On3SpiderMVC.Infrastructure;
using On3SpiderMVC.ViewModels;
using Spider;

namespace On3SpiderMVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var model = new FileUploadViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(FileUploadViewModel model)
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);

                    if (!String.IsNullOrWhiteSpace(fileName))
                    {
                        var uploadPath = Infrastructure.Constants.UploadFileLocation;
                        var path = Path.Combine(Server.MapPath(uploadPath), fileName);
                        file.SaveAs(path);
                    }


                }
            }

            return View(model);
        }


    }
}