using Crawler.Helpers;
using Crawler.HtmlReaders;
using Crawler.ItemReaders;
using Crawler.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Crawler.PageReaders
{
    public class TotalPageReader : IPageReader
    {
        private SiteParameter siteParameter;
        private IHtmlReader htmlReader;
        private IItemReader itemReader;
        private int pageNumber = -1;
        private List<Tuple<string, string, int>> maps = new List<Tuple<string, string, int>>() {
            new Tuple<string, string, int>("Thailand", "<INPUT id=totalNumberOfResult type=hidden value=(.+?)>", 10),
            new Tuple<string, string, int>("China", "<input type=\"hidden\" value=\"(.+?)\" id=\"totalNumberOfResult\" \\/>", 10),
            new Tuple<string, string, int>("Malaysia", "<input type=\"hidden\" value=\"(.+?)\" id=\"totalNumberOfResult\" \\/>", 10)
        };

        public TotalPageReader(SiteParameter siteParameter, IHtmlReader htmlReader, IItemReader itemReader)
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
            int total = 0;
            Tuple<string, string, int> current = maps.FirstOrDefault(f => f.Item1 == this.siteParameter.CountryPattern);
            if (this.siteParameter.CountryPattern.Equals("Thailand") && this.siteParameter.MerchantNamePattern.Equals("屈臣氏"))
            {
                var html = this.htmlReader.GetHtml("https://www.watsons.co.th/_s/language?code=en");
            }

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

                total = Convert.ToInt32(Regex.Match(html, current.Item2).Groups[1].Value);
                articles = this.itemReader.GetShops(html, this.siteParameter.StartUrl).ToArray();

                foreach (var article in articles)
                {
                    LogHelper.WriteInfo($"Getting {article.SiteUrl}");
                    yield return article;
                }
            }

            if (!string.IsNullOrWhiteSpace(this.siteParameter.SiteUrlPattern))
            {
                do
                {
                    string url = string.Format(this.siteParameter.SiteUrlPattern, this.pageNumber);
                    this.pageNumber += this.siteParameter.PageStepNumber ?? 1;
                    LogHelper.WriteInfo($"Parsing {url}");

                    string html = null;
                    try
                    {
                        html = this.htmlReader.GetHtml(url);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteError($"Request {url} error.", ex);
                    }

                    if (string.IsNullOrWhiteSpace(html))
                    {
                        yield break;
                    }

                    if (Regex.IsMatch(html, current.Item2))
                    {
                        total = Convert.ToInt32(Regex.Match(html, current.Item2).Groups[1].Value);
                    }
                    articles = this.itemReader.GetShops(html, url).ToArray();

                    foreach (var article in articles)
                    {
                        LogHelper.WriteInfo($"Getting {article.SiteUrl}");
                        yield return article;
                    }
                }
                while ((this.pageNumber - 1) * current.Item3 < total);
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
