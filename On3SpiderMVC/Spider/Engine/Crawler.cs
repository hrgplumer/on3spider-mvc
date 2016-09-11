using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Abot.Crawler;
using Abot.Poco;
using Abot.Util;
using AbotX.Crawler;
using AbotX.Parallel;
using AbotX.Poco;
using log4net;
using log4net.Config;
using Spider.Interface;

namespace Spider.Engine
{
    /// <summary>
    /// Wrapper class for AbotX ParallelCrawlerEngine that handles configuration, startup, shutdown
    /// </summary>
    public class Crawler : ICrawler
    {
        private readonly ParallelCrawlerEngine _crawler;

        /// <summary>
        /// Initializes the AbotX parallel crawler using app.config settings, to crawl the list of urls provided.
        /// </summary>
        /// <param name="urls">The list of urls the Crawler should visit.</param>
        public Crawler(IEnumerable<string> urls)
        {
            // configure log4net -- this must be called before crawler is created
            XmlConfigurator.Configure();

            var provider = new SiteToCrawlProvider();
            provider.AddSitesToCrawl(urls.Select(url =>
            new SiteToCrawl()
            {
                Uri = new Uri(url)
            }));

            _crawler = new ParallelCrawlerEngine(provider);
        }

        /// <summary>
        /// Begins crawling pages asynchronously.
        /// </summary>
        public void Crawl()
        {
            _crawler.StartAsync();
        }

        /// <summary>
        /// Stops the crawl.
        /// </summary>
        /// <param name="isHardStop">(Optional) Determins whether this crawl stop is a hard stop. 
        ///  A hard stop will stop immediately, a soft one will wait for any current crawls to finish.</param>
        public void StopCrawl(bool isHardStop = false)
        {
            _crawler.Stop(isHardStop);
        }

        #region Event Handlers

        // TODO: determine if we need the Async events here, or if regular synchronous events are fine

        /// <summary>
        /// Event fired when all crawls in the crawl queue have completed.
        /// </summary>
        public event EventHandler<AllCrawlsCompletedArgs> AllCrawlsCompleted
        {
            add { _crawler.AllCrawlsCompleted += value; }
            remove { _crawler.AllCrawlsCompleted -= value; }
        }

        /// <summary>
        /// Event fired when crawl has completed for one site.
        /// </summary>
        public event EventHandler<SiteCrawlCompletedArgs> SiteCrawlCompleted
        {
            add { _crawler.SiteCrawlCompleted += value; }
            remove { _crawler.SiteCrawlCompleted -= value; }
        }

        public event EventHandler<CrawlerInstanceCreatedArgs> CrawlerInstanceCreated
        {
            add { _crawler.CrawlerInstanceCreated += value; }
            remove { _crawler.CrawlerInstanceCreated -= value; }
        }

        #endregion
    }
}
