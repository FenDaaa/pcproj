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
    public class ParamSequentialPageReader: IPageReader
    {
        private SiteParameter siteParameter;
        private IHtmlReader htmlReader;
        private IItemReader itemReader;
        private int pageNumber = -1;

        public ParamSequentialPageReader(SiteParameter siteParameter, IHtmlReader htmlReader, IItemReader itemReader)
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

            if (!string.IsNullOrWhiteSpace(this.siteParameter.SiteUrlPattern))
            {
                Tuple<string, string, int> param = this.siteParameter.UrlParams.First(s => s.Item3 == 1);
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
                List<string> whileParams = MatchedValues(param.Item2, document).ToList();
                int index = 0;
                do
                {
                    string urlPattern = this.siteParameter.SiteUrlPattern.Replace(string.Format("{{{0}}}", param.Item1), whileParams[index].ToString());
                    do
                    {
                        string url = string.Format(urlPattern, this.pageNumber);
                        this.pageNumber += this.siteParameter.PageStepNumber ?? 1;
                        LogHelper.WriteInfo($"Parsing {url}");

                        string each = null;
                        for (int i = 0; i < 3; i++)
                        {
                            try
                            {
                                each = this.htmlReader.GetHtml(url);
                            }
                            catch (Exception ex)
                            {
                                LogHelper.WriteError($"Request {url} error.", ex);
                            }
                            if(!string.IsNullOrWhiteSpace(each))
                            {
                                break;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(each))
                        {
                            yield break;
                        }

                        articles = this.itemReader.GetShops(each, url).ToArray();

                        foreach (var article in articles)
                        {
                            LogHelper.WriteInfo($"Getting {article.SiteUrl}");
                            yield return article;
                        }
                    }
                    while (articles.Count() > 0);
                    index++;
                    this.pageNumber = this.siteParameter.StartNumber;
                } while (index < whileParams.Count);
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
