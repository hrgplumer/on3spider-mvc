using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using On3SpiderMVC.Infrastructure;
using On3SpiderMVC.ViewModels;
using On3SpiderMVC.Repository;
using Spider.Models;
using Spider.Interface;
using System.Threading.Tasks;
using Spider.Engine;
using Abot.Poco;
using On3SpiderMVC.ViewModels.Home;
using Spider.Infrastructure.Excel;

namespace On3SpiderMVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var catRepo = new CategoryRepository();
            var model = new FileUploadViewModel()
            {
                CategoryDropDown = catRepo.GetFileCategories().Select(category => new SelectListItem()
                {
                    Text = category,
                    Value = category
                })
            };
            return View(model);
        }

        /// <summary>
        /// Entry point for application: Capture uploaded file and category and start processing
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Index(FileUploadViewModel model)
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);

                    if (!String.IsNullOrWhiteSpace(fileName))
                    {
                        var uploadPath = Constants.UploadFileLocation;
                        var path = Path.Combine(Server.MapPath(uploadPath), fileName);
                        file.SaveAs(path);

                        // file is uploaded to server; now open it with excel reader
                        var reader = new ExcelReader<RosterSheet>(path);
                        var urls = reader.ReadSheet().ToList();
                        var category = model.CategoryDropDownSelection;

                        if (!urls.Any())
                        {
                            // no URLs in spreadsheet -- error
                            return View("Results", new ResultsViewModel()
                            {
                                Error = "No URLs found in selected spreadsheet."
                            });
                        }

                        if (String.IsNullOrWhiteSpace(category))
                        {
                            // no category selected -- error
                            return View("Results", new ResultsViewModel()
                            {
                                Error = "No Category selected."
                            });
                        }

                        // Create info dictionary to propagate original info from spreadsheet thru the app
                        var urlInfoDict = urls.ToDictionary<RosterSheet, string, ISheetRow>(row => row.Url, row => row);

                        // Start the crawling engine on a new thread
                        var crawlResults = await Task.Run(() => StartCrawlingEngineAsync(urlInfoDict, category));

                        return View("Results", new ResultsViewModel()
                        {
                            Error = "Success",
                            ResultsList = crawlResults
                        });
                    }
                }
            }

            // if we make it here without returning, something weird happened
            return View("Results", new ResultsViewModel()
            {
                Error = "Unspecified error."
            });
        }


        #region Privates

        /// <summary>
        /// Starts the crawling engine.
        /// </summary>
        /// <param name="urlDictionary">The list of urls to crawl.</param>
        /// <param name="category">The category of these urls.</param>
        private async Task<IList<ISheetRow>>  StartCrawlingEngineAsync(Dictionary<string, ISheetRow> urlDictionary, string category)
        {
            var manager = new EngineManager(new Crawler(urlDictionary.Keys), category, urlDictionary, new QueueManager<CrawledPage>());
            var result = await manager.StartAsync();

            return result;
        }

        #endregion


    }
}