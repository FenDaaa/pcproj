using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Crawler.Helpers;
using Crawler.HtmlReaders;
using Crawler.ItemReaders;
using Crawler.Models;
using HtmlAgilityPack;

namespace Crawler.PageReaders
{
    class PumaPageReader : IPageReader
    {
        protected HttpClient client = new HttpClient();
        private SiteParameter siteParameter;
        private IHtmlReader htmlReader;
        private IItemReader itemReader;
        private int pageNumber = -1;
        private string[] countryCode = new string[] { "AF", "AX", "AL", "DZ", "AS", "AD", "AO", "AI", "AQ", "AG", "AR", "AM", "AW", "AU", "AT", "AZ", "BS", "BH", "BD", "BB", "BY", "BE", "BZ", "BJ", "BM", "BT", "BO", "BQ", "BA", "BW", "BV", "BR", "IO", "BN", "BG", "BF", "BI", "KH", "CM", "CA", "CV", "KY", "CF", "TD", "CL", "CX", "CC", "CO", "KM", "CD", "CG", "CK", "CR", "CI", "HR", "CU", "CW", "CY", "CZ", "DK", "DJ", "DM", "DO", "EC", "EG", "SV", "GQ", "ER", "EE", "ET", "FK", "FO", "FJ", "FI", "FR", "GF", "PF", "TF", "GA", "GM", "GE", "DE", "GH", "GI", "GR", "GL", "GD", "GP", "GU", "GT", "GG", "GW", "GN", "GY", "HT", "HM", "VA", "HN", "HK", "HU", "IS", "IN", "ID", "IR", "IQ", "IE", "IM", "IL", "JM", "JP", "JE", "JO", "KZ", "KE", "KI", "KP", "KR", "KW", "KG", "LA", "LV", "LB", "LS", "LR", "LY", "LI", "LT", "LU", "MO", "MK", "MG", "MW", "MY", "MV", "ML", "MT", "MH", "MQ", "MR", "MU", "YT", "MX", "FM", "MD", "MC", "MN", "ME", "MS", "MA", "MZ", "MM", "NA", "NR", "NP", "NL", "NC", "NZ", "NI", "NG", "NE", "NU", "NF", "MP", "NO", "OM", "PK", "PW", "PS", "PA", "PG", "PY", "PE", "PH", "PN", "PL", "PT", "PR", "QA", "RE", "RO", "RU", "RW", "BL", "SH", "KN", "LC", "MF", "PM", "VC", "WS", "SM", "ST", "SA", "SN", "RS", "SC", "SL", "SG", "SX", "SK", "SI", "SB", "SO", "ZA", "GS", "SS", "ES", "LK", "SD", "SR", "SJ", "SZ", "SE", "CH", "SY", "TW", "TJ", "TZ", "TH", "TL", "TG", "TK", "TO", "TT", "TN", "TR", "TM", "TC", "TV", "UG", "UA", "AE", "GB", "UM", "US", "UY", "UZ", "VU", "VE", "VN", "VG", "VI", "WF", "EH", "YE", "ZM", "ZW" };
        //private string[] countryCode = new string[] { "AF", "AX", "AL", "DZ", "AS", "AD", "AO", "AI", "AQ", "AG", "AR", "AM", "AW", "HK" };
        //private string[] countryCode = new string[] { "CN" };

        public PumaPageReader(SiteParameter siteParameter, IHtmlReader htmlReader, IItemReader itemReader)
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
            //经纬度集合
            List<string> lonLats = new List<string>();
            if (!string.IsNullOrWhiteSpace(this.siteParameter.SiteUrlPattern))
            {
                Tuple<string, string, int> param = this.siteParameter.UrlParams.First(s => s.Item3 == 1);
                string html = null;
                string newurls = null;
                try
                {
                    foreach(var countryCodeItem in countryCode)
                    {
                        newurls = this.siteParameter.StartUrl.Replace("{0}", countryCodeItem);
                        html = this.htmlReader.GetHtml(newurls);
                        HtmlDocument document = new HtmlDocument();
                        document.LoadHtml(html);
                        var whileParams = MatchedValues(param.Item2, document).Distinct().ToList();
                        lonLats.AddRange(whileParams);
                        LogHelper.WriteInfo($"Parsing {newurls}");
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteError($"Request {this.siteParameter.StartUrl} error.", ex);
                }
            }
            foreach (var lonlatsItem in lonLats)
            {
                string pageDetailJson = GetHtmlByPost(this.siteParameter.SiteUrlPattern, lonlatsItem);
                articles = this.itemReader.GetShops(pageDetailJson, this.siteParameter.SiteUrlPattern).ToArray();
                LogHelper.WriteInfo($"Parsing {this.siteParameter.SiteUrlPattern}");
                foreach (var article in articles)
                {
                    yield return article;
                }
            }
        }

        public List<string> MatchedValues(string pattern, HtmlDocument document)
        {
            var result = string.Empty;
            if (!pattern.StartsWith("//"))
            {
                return Regex.Matches(document.Text, pattern)?.Cast<Match>().Select(s => s.Groups[1].Value.TrimContent()+","+ s.Groups[2].Value.TrimContent()).ToList();
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

        public virtual string GetHtmlByPost(string url, string lonLats)
        {
            try
            {
                string lat = lonLats.Split(',').First();
                string lon = lonLats.Split(',').Last();
                var content = new FormUrlEncodedContent(new Dictionary<string, string>()
                    {
                        {"LA", lat},
                        {"LO", lon},
                        {"RFR", url},
                        {"ITER", "site-au"},
                        {"STYPE", "GROUP"},
                        {"CLIENT", "puma"}
                    });
                var response = this.client.PostAsync(url, content).Result;
                var html = response.Content.ReadAsStringAsync().Result;
                return HttpUtility.HtmlDecode(html);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
    }
}
