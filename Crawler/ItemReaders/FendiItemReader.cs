// <copyright file="RegexItemReader.cs" company="pactera.com">
//     pactera.com. All rights reserved.
// </copyright>

namespace Crawler.ItemReaders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Crawler.Helpers;
    using Crawler.Models;

    public class FendiItemReader : IItemReader
    {
        private SiteParameter siteParameter;
        private Regex pattern;

        public FendiItemReader(SiteParameter siteParameter)
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
            Match match = Regex.Match(current.Value, "{\"name\":.*?displayName\":\"(.*?)\",\"url\":\".*?\",.*?geoPoint\":{\"latitude\":(.*?),\"longitude\":(.*?)},.*?town\":\"(.*?)\",.*?country\":{.*?name\":\"(.*?)\",.*?},.*?formattedAddress\":\"(.*?)\",.*?type\":\"(.*?)\"");
            List<int> indexs = this.siteParameter.JsonIndexs.Split(',').Select(s => int.Parse(s)).ToList();
            if (indexs[1] > 0)
            {
                shop.SubbranchName = match.Groups[indexs[1]].Value.TrimUnicode();
            }

            if (indexs[2] > 0)
            {
                shop.Country = match.Groups[indexs[2]].Value;
            }

            if (indexs[3] > 0)
            {
                shop.City = match.Groups[indexs[3]].Value;
            }

            if (indexs[4] > 0)
            {
                shop.Address = match.Groups[indexs[4]].Value.TrimContent().TrimDoubleQuote().TrimLine().TrimEscape();
            }

            if (indexs[5] > 0)
            {
                shop.Longitude = match.Groups[indexs[5]].Value.TrimDoubleQuote();
            }

            if (indexs[6] > 0)
            {
                shop.Latitude = match.Groups[indexs[6]].Value.TrimDoubleQuote();
            }

            if (indexs[7] > 0)
            {
                //string hourStr = match.Groups[indexs[7]].Value;
                if (Regex.IsMatch(current.Value, "{\"openingTime\".*?\"formattedHour\":\"(.*?)\".*?\"formattedHour\":\"(.*?)\".*?\"weekDay\":\"(.*?)\","))
                {
                    List<Match> list = Regex.Matches(current.Value, "{\"openingTime\".*?\"formattedHour\":\"(.*?)\".*?\"formattedHour\":\"(.*?)\".*?\"weekDay\":\"(.*?)\",").Cast<Match>().ToList();
                    StringBuilder sb = new StringBuilder();
                    list.ForEach(l => {
                        sb.Append(string.Format("{0}:{1}-{2},", l.Groups[3].Value, l.Groups[1].Value, l.Groups[2].Value));
                    });
                    shop.OpenHours = sb.ToString().Trim(',');
                }
            }

            if (indexs[8] > 0)
            {
                if (Regex.IsMatch(current.Value, "phone\":\"(.*?)\","))
                {
                    shop.Telphone = Regex.Match(current.Value, "phone\":\"(.*?)\",").Groups[1].Value;
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
                if (!string.IsNullOrWhiteSpace(this.siteParameter.SiteUrlPattern) && Regex.IsMatch(this.siteParameter.SiteUrlPattern, "{\\D+}"))
                {
                    shop.SiteUrl = Regex.Replace(this.siteParameter.SiteUrlPattern, "{\\D+}", match.Groups[indexs[12]].Value, RegexOptions.IgnoreCase).UrlDecode();
                    if (match.Groups[indexs[12]].Value.Contains("http"))
                    {
                        shop.SiteUrl = match.Groups[indexs[12]].Value;
                    }
                }
                else
                {
                    shop.SiteUrl = match.Groups[indexs[12]].Value.ToAbsoluteUrl(pageUrl);
                }
                if (this.siteParameter.MerchantNamePattern.Equals("华伦天奴"))
                {
                    shop.SiteUrl = string.Format("{0},{1}", match.Groups[8].Value, match.Groups[indexs[12]].Value);
                }
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
