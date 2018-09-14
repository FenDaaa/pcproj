
namespace Crawler.ItemReaders
{
    using Crawler.Helpers;
    using Crawler.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class YSLGlobalItemReader : IItemReader
    {
        private SiteParameter siteParameter;
        private Regex pattern;

        public YSLGlobalItemReader(SiteParameter siteParameter)
        {
            this.siteParameter = siteParameter ?? throw new ArgumentNullException(nameof(siteParameter));
            this.pattern = new Regex(siteParameter.ItemPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        }

        public IEnumerable<Shop> GetShops(string html, string pageUrl)
        {
            List<Match> matches = this.pattern.Matches(html).Cast<Match>().ToList();
            List<Match> list = new List<Match>();
            IEnumerable<Shop> shops = matches
                .Select(match =>
                {
                    {
                        Shop shop = FormatShop(match, pageUrl);
                        return shop;
                    }
                });
            return shops;
        }

        public Shop FormatShop(Match current, string pageUrl)
        {
            Shop shop = new Shop();
            Match match = Regex.Match(current.Value, "\"post_title\":\"(.+?)\"[\\s\\S]*?\"permalink\":\"(.+?)\"[\\s\\S]*?\"wpcf-yoox-store-collection\":\"(.+?)\"[\\s\\S]*?\"wpcf-yoox-store-address\":\"(.+?)\"[\\s\\S]*?\"wpcf-yoox-store-hours\":\"([\\s\\S]+?)\"");
            List<int> indexs = this.siteParameter.JsonIndexs.Split(',').Select(s => int.Parse(s)).ToList();
            if (indexs[1] > 0)
            {
                shop.SubbranchName = match.Groups[indexs[1]].Value.TrimUnicode();
            }

            if (indexs[2] > 0)
            {
                if (Regex.IsMatch(current.Value, "country\":{.*?name\":\"(.*?)\","))
                {
                    shop.Country = Regex.Match(current.Value, "country\":{.*?name\":\"(.*?)\",").Groups[1].Value;
                }
            }

            if (indexs[3] > 0)
            {
                if (Regex.IsMatch(current.Value, "\"wpcf-yoox-store-city\":\"(.+?)\""))
                {
                    shop.City = Regex.Match(current.Value, "\"wpcf-yoox-store-city\":\"(.+?)\"").Groups[1].Value;
                }
            }

            if (indexs[4] > 0)
            {
                shop.Address = match.Groups[indexs[4]].Value.TrimContent().TrimDoubleQuote().TrimLine().TrimEscape();
            }

            if (indexs[5] > 0)
            {
                if (Regex.IsMatch(current.Value, "\"lng\":\"(.*?)\""))
                {
                    shop.Longitude = Regex.Match(current.Value, "\"lng\":\"(.*?)\"").Groups[1].Value;
                }

                //shop.Longitude = match.Groups[indexs[5]].Value.TrimDoubleQuote();
            }

            if (indexs[6] > 0)
            {
                if (Regex.IsMatch(current.Value, "\"lat\":\"(.*?)\""))
                {
                    shop.Latitude = Regex.Match(current.Value, "\"lat\":\"(.*?)\"").Groups[1].Value;
                }

                //shop.Latitude = match.Groups[indexs[6]].Value.TrimDoubleQuote();
            }

            if (indexs[7] > 0)
            {
                shop.OpenHours = match.Groups[indexs[7]].Value.TrimContent().TrimDoubleQuote().TrimLine().TrimUnicode();
            }

            if (indexs[8] > 0)
            {
                if (Regex.IsMatch(current.Value, "\"wpcf-yoox-store-phone\":\"(.+?)\""))
                {
                    shop.Telphone = Regex.Match(current.Value, "\"wpcf-yoox-store-phone\":\"(.+?)\"").Groups[1].Value;
                }
            }

            if (indexs[9] > 0)
            {
                shop.ShopType = match.Groups[indexs[9]].Value;
            }

            if (indexs[11] > 0)
            {
                shop.Picture = match.Groups[indexs[11]].Value;
            }

            if (indexs[12] > 0)
            {
                shop.SiteUrl = match.Groups[indexs[12]].Value.TrimContent().TrimDoubleQuote();
            }

            if (indexs[13] > 0)
            {
                string matchValue = Regex.Replace(match.Groups[indexs[13]].Value, "<span class=\"\">.+?</span>", string.Empty, RegexOptions.IgnoreCase).Trim();
                shop.Scope = Regex.Replace(matchValue, "<span.+?>", string.Empty, RegexOptions.IgnoreCase).Replace("</span>", ",").TrimContent();
            }
            return shop;
        }

    }

}
