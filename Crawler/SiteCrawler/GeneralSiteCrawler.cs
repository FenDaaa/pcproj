// <copyright file="GeneralSiteCrawler.cs" company="pactera.com">
//     pactera.com. All rights reserved.
// </copyright>

namespace Crawler.SiteCrawler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Crawler.Helpers;
    using Crawler.HtmlReaders;
    using Crawler.ItemReaders;
    using Crawler.Models;
    using Crawler.PageParsers;
    using Crawler.PageReaders;
    using NPOI.XSSF.UserModel;

    public class GeneralSiteCrawler : ISiteCrawler
    {
        private IPageReader pageReader;
        private IPageParser pageParser;

        public GeneralSiteCrawler(IPageReader pageReader, IPageParser pageParser)
        {
            this.pageReader = pageReader ?? throw new ArgumentNullException(nameof(pageReader));
            this.pageParser = pageParser ?? throw new ArgumentNullException(nameof(pageParser));
        }

        public GeneralSiteCrawler(SiteParameter siteParameter)
        {
            IItemReader itemReader = new RegexItemReader(siteParameter);

            IHtmlReader htmlReader = new HttpClientReader();

            this.pageReader = new SequentialPageReader(siteParameter, htmlReader, itemReader);
            this.pageParser = new RegexPageParser(siteParameter, htmlReader);
        }

        public void Crawl()
        {
            IEnumerable<Shop> shops = this.pageReader.GetShops().ToArray();
            shops = shops.Select(article => this.pageParser.GetShopDetails(article)).ToArray();
            if (shops.Any())
            {
                WriteExcel(shops);
            }
        }

        public void WriteExcel(IEnumerable<Shop> shops)
        {
            XSSFWorkbook work = NPOIHelper.BuildWorkbook(shops.ToList());
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            work.Write(ms);
            using (FileStream fs = new FileStream(string.Format("D:\\{0}-{1}-{2}.xlsx", shops.First().MerchantName, shops.First().Country, shops.First().City), FileMode.OpenOrCreate))
            {
                byte[] data = ms.ToArray();
                fs.Write(data, 0, data.Length);
                fs.Flush();
            }
            ms.Close();
            ms.Dispose();
        }
    }
}
