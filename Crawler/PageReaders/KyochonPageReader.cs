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
    public class KyochonPageReader : IPageReader
    {
        private SiteParameter siteParameter;
        private IHtmlReader htmlReader;
        private IItemReader itemReader;
        private int pageNumber = -1;

        public KyochonPageReader(SiteParameter siteParameter, IHtmlReader htmlReader, IItemReader itemReader)
        {
            this.siteParameter = siteParameter ?? throw new ArgumentNullException(nameof(siteParameter));
            this.htmlReader = htmlReader ?? throw new ArgumentNullException(nameof(htmlReader));
            this.itemReader = itemReader ?? throw new ArgumentNullException(nameof(itemReader));
            this.pageNumber = this.siteParameter.StartNumber;
        }

        public IEnumerable<Shop> GetShops()
        {
            IEnumerable<Shop> articles = null;
            //1.抓取本地数据
            if (!string.IsNullOrWhiteSpace(this.siteParameter.StartUrl))
            {
                LogHelper.WriteInfo($"Parsing {this.siteParameter.StartUrl}");
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

                articles = this.itemReader.GetShops(html, this.siteParameter.StartUrl).ToArray();

                foreach (var article in articles)
                {
                    LogHelper.WriteInfo($"Getting {article.SiteUrl}");
                    yield return article;
                }
            }
            //2.抓取海外数据
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
