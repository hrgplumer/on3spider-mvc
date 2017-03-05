using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abot.Crawler;
using Abot.Poco;
using AbotX.Parallel;
using Spider.Infrastructure;
using Spider.Interface;

namespace Spider.Engine
{
    /// <summary>
    /// Manager class for all SpiderEngine tasks.
    /// </summary>
    public class EngineManager
    {
        private readonly ICrawler _crawler;
        private readonly IQueueManager<CrawledPage> _queue;
        private readonly String _category;
        private readonly Dictionary<string, ISheetRow> _urlDictionary;

        // this should be moved out to App.config
        private readonly int PageAnalyzeThreshold;

        /// <summary>
        /// Volatile flag used by the page crawling thread to signal that it is finished processing.
        /// </summary>
        private volatile bool _allCrawlsCompleted = false;

        /// <summary>
        /// Create a new EngineManager using the given crawler and queue.
        /// </summary>
        /// <param name="crawler">An ICrawler instance. This will be used as the web crawler.</param>
        /// <param name="category"></param>
        /// <param name="queue">An IQueueManager instance. This will be used as the thread safe queue for processing crawled pages.</param>
        public EngineManager(ICrawler crawler, string category, Dictionary<string, ISheetRow> urlDict, IQueueManager<CrawledPage> queue)
        {
            if (crawler == null)
                throw new ArgumentNullException(nameof(crawler));
            if (category == null)
                throw new ArgumentNullException(nameof(category));
            if (urlDict == null)
                throw new ArgumentNullException(nameof(urlDict));
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

            _crawler = crawler;
            _category = category;
            _urlDictionary = urlDict;
            _queue = queue;

            // Set page analyze threshold to value in app.config, OR the URL count if URL count is less
            try
            {
                var threshold = Convert.ToInt32(ConfigurationManager.AppSettings[Constants.PageAnalyzeThresholdSettingName]);
                PageAnalyzeThreshold = _urlDictionary.Count < threshold ? _urlDictionary.Count : threshold;
            }
            catch (FormatException ex)
            {
                throw new Exception("Could not parse PageAnalyzeThreshold setting in app.config. Make sure this setting exists.", ex);
            }
            catch (OverflowException ex)
            {
                throw new Exception("PageAnalyzeThreshold setting in app.config caused integer overflow. Lower this setting to Int32 range.", ex);
            }
            catch (ConfigurationErrorsException ex)
            {
                throw new Exception("Configuration error encountered when retrieving PageAnalyzeThreshold setting in app.config. Check this setting.", ex);
            }

            RegisterCrawlerEvents();
        }

        /// <summary>
        /// Starts the web crawl.
        /// </summary>
        public async Task<IList<ISheetRow>>  StartAsync()
        {
            _crawler.Crawl();
            using (var processTask = ProcessCrawledPagesAsync())
            {
                await processTask;

                return processTask.Result;
            }
        }

        /// <summary>
        /// Processes the crawled page results in a separate thread. 
        /// </summary>
        /// <returns>A list of results, or null if something went wrong.</returns>
        private async Task<IList<ISheetRow>> ProcessCrawledPagesAsync()
        {
            var stack = new Stack<CrawledPage>();
            var pageResults = new ConcurrentBag<ISheetRow>();

            while (!_allCrawlsCompleted || !_queue.IsEmpty())
            {
                CrawledPage page = null;
                if (_queue.TryDequeue(out page))
                {
                    if (page == null) continue;

                    Trace.WriteLine($"Dequeued page {page.Uri.AbsoluteUri}: queue has {_queue.Count()} elements remaining");

                    // add dequeued page to stack
                    stack.Push(page);

                    /* if we've reached the threshold of pages we want to process in a single thread,
                     create the thread and process it */
                    if (stack.Count >= PageAnalyzeThreshold)
                    {
                        var stackItems = stack.ToArray();
                        stack.Clear();
                        Trace.WriteLine("cleared stack");
                        var results = await ProcessPage(stackItems);

                        // add results to thread-safe bag
                        foreach (var result in results)
                        {
                            pageResults.Add(result);
                        }
                    }
                }
            }

            return pageResults.ToList();
        }

        /// <summary>
        /// Calls a PageAnalyzer on the crawled page results.
        /// </summary>
        /// <param name="pages"></param>
        /// <returns></returns>
        private async Task<IList<ISheetRow>> ProcessPage(IEnumerable<CrawledPage> pages)
        {
            var analyzer = new PageAnalyzer(pages, _category, _urlDictionary);
            var result = await analyzer.AnalyzeAsync();

            return result;
        }

        /// <summary>
        /// Stops the web crawl.
        /// <param name="isHardStop">Determines whether the stop is a hard stop (immediately stops all threads)
        ///  or soft stop (waits for current processing to end, then stops)</param>
        /// </summary>
        public void Stop(bool isHardStop)
        {
            _crawler.StopCrawl(isHardStop);
        }

        #region Event Handlers

        public void Crawler_AllCrawlsCompleted(object sender, AllCrawlsCompletedArgs args)
        {
            _allCrawlsCompleted = true;
            Trace.WriteLine($"Completed crawling all sites, queue has {_queue.Count()} elements remaining");
        }

        public void Crawler_SiteCrawlCompleted(object sender, SiteCrawlCompletedArgs args)
        {
            Trace.WriteLine($"Completed crawling site {args.CrawledSite.SiteToCrawl.Uri}");
        }

        public void Crawler_CrawlerInstanceCreated(object sender, CrawlerInstanceCreatedArgs args)
        {
            // register for crawler level events. this is an abot event
            args.Crawler.PageCrawlCompleted += Crawler_PageCrawlCompleted;
        }

        public void Crawler_PageCrawlCompleted(object sender, PageCrawlCompletedArgs args)
        {
            var crawledPage = args.CrawledPage;

            if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
            {
                // an exception of some sort occured while crawling
                Trace.WriteLine($"Crawl of page failed {crawledPage.Uri.AbsoluteUri}");
            }
            else
            {
                Trace.WriteLine($"Crawl of page succeeded {crawledPage.Uri.AbsoluteUri}");

                // add crawled page to queue
                _queue.Enqueue(crawledPage);
                Trace.WriteLine($"Queue contains {_queue.Count()} items");
            }

            if (string.IsNullOrEmpty(crawledPage.Content.Text))
            {
                Trace.WriteLine($"Page had no content {crawledPage.Uri.AbsoluteUri}");
            }
        }

        #endregion

        #region Privates

        private void RegisterCrawlerEvents()
        {
            _crawler.AllCrawlsCompleted += Crawler_AllCrawlsCompleted;
            _crawler.SiteCrawlCompleted += Crawler_SiteCrawlCompleted;
            _crawler.CrawlerInstanceCreated += Crawler_CrawlerInstanceCreated;
        }

        #endregion
    }
}
