using Crawler.Helpers;
using Crawler.HtmlReaders;
using Crawler.ItemReaders;
using Crawler.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Crawler.PageReaders
{
    public class ValentinoPageReader : IPageReader
    {
        private SiteParameter siteParameter;
        private IHtmlReader htmlReader;
        private IItemReader itemReader;
        private int pageNumber = -1;

        public ValentinoPageReader(SiteParameter siteParameter, IHtmlReader htmlReader, IItemReader itemReader)
        {
            this.siteParameter = siteParameter ?? throw new ArgumentNullException(nameof(siteParameter));
            this.htmlReader = htmlReader ?? throw new ArgumentNullException(nameof(htmlReader));
            this.itemReader = itemReader ?? throw new ArgumentNullException(nameof(itemReader));
            this.pageNumber = this.siteParameter.StartNumber;
        }

        public IEnumerable<Shop> GetShops()
        {
            IEnumerable<Shop> articles = null;
            List<string> urls = new List<string>();
            string html = null;
            try
            {
                html = this.htmlReader.GetHtml(this.siteParameter.StartUrl);
            }
            catch (Exception ex)
            {
                LogHelper.WriteError($"Request {this.siteParameter.StartUrl} error.", ex);
            }

            if (string.IsNullOrWhiteSpace(html))
            {
                yield break;
            }

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            List<Match> list = Regex.Matches(html, "<li class=\"(.*?)\">\\s+?<a href=\"(.+?)\".*?>\\s+?<span.*?>.*?<\\/span>\\s+?<span.*?>.*?<\\/span>").Cast<Match>().ToList();
            string url = string.Format("https://ecomm.ynap.biz/api/os2//wcs/resources/store/valentino_{0}/storelocator/boutiques?pageSize=500&t=1", list.First().Groups[1].Value);
            LogHelper.WriteInfo($"Parsing {url}");

            string current = null;
            try
            {
                current = this.htmlReader.GetHtml(url);
            }
            catch (Exception ex)
            {
                LogHelper.WriteError($"Request {url} error.", ex);
            }

            if (string.IsNullOrWhiteSpace(current))
            {
                yield break;
            }

            articles = this.itemReader.GetShops(current, url).Where(s => s.Country != "China").ToArray();

            foreach (var article in articles)
            {
                string[] values = article.SiteUrl.Split(',');
                string baseUrl = list.FirstOrDefault(f => f.Groups[1].Value == values[0].ToLower())?.Groups[2].Value;
                if (string.IsNullOrWhiteSpace(baseUrl)) { baseUrl = "https://www.valentino.com/en-US"; }
                article.SiteUrl = string.Format("{0}/store-locator/#store/{1}", baseUrl, values[1]);
                yield return article;
            }
        }

        public List<string> MatchedValues(string pattern, HtmlDocument document)
        {
            var result = string.Empty;
            if (!pattern.StartsWith("//"))
            {
                return Regex.Matches(document.Text, pattern)?.Cast<Match>().Select(s => s.Groups[1].Value.TrimContent()).ToList();
            }
            else
            {
                var contentNode = document.DocumentNode.SelectNodes(pattern);
                return contentNode?.Select(s => s.TrimScript().OuterHtml.TrimContent()).ToList();
            }
        }

        public string MatchedValue(string pattern, HtmlDocument document)
        {
            var result = string.Empty;
            if (!pattern.StartsWith("//"))
            {
                return Regex.Match(document.Text, pattern)?.Groups[1].Value.TrimContent();
            }
            else
            {
                var contentNode = document.DocumentNode.SelectSingleNode(pattern);
                return contentNode?.TrimScript().OuterHtml.TrimContent();
            }
        }
    }
}
