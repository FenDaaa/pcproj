using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Crawler.Helpers;
using Crawler.Models;

namespace Crawler.ItemReaders
{
    class PumaItemReader : IItemReader
    {
        private SiteParameter siteParameter;
        private Regex pattern;

        public PumaItemReader(SiteParameter siteParameter)
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

        public Shop FormatShop(Match match, string pageUrl)
        {
            Shop shop = new Shop();
            List<int> indexs = this.siteParameter.JsonIndexs.Split(',').Select(s => int.Parse(s)).ToList();
            if (indexs[1] > 0)
            {
                if (Regex.IsMatch(match.Value, "\"name\":\"(.*?)\","))
                {
                    shop.SubbranchName = Regex.Match(match.Value, "\"name\":\"(.*?)\",").Groups[1].Value;
                }
            }

            if (indexs[2] > 0)
            {
                if (Regex.IsMatch(match.Value, "\"country\":\"(.*?)\","))
                {
                    shop.Country = Regex.Match(match.Value, "\"country\":\"(.*?)\",").Groups[1].Value;
                }
            }

            if (indexs[3] > 0)
            {
                if (Regex.IsMatch(match.Value, "\"city\":\"(.*?)\","))
                {
                    shop.City = Regex.Match(match.Value, "\"city\":\"(.*?)\",").Groups[1].Value;
                }
            }

            if (indexs[4] > 0)
            {
                if (Regex.IsMatch(match.Value, "\"add1\":\"(.*?)\","))
                {
                    shop.Address = Regex.Match(match.Value, "\"add1\":\"(.*?)\",").Groups[1].Value.TrimUnicode().TrimEscape();
                }
            }

            if (indexs[5] > 0)
            {
                if (Regex.IsMatch(match.Value, "\"lng\":(.*?),"))
                {
                    shop.Longitude = Regex.Match(match.Value, "\"lng\":(.*?),").Groups[1].Value;
                }
            }

            if (indexs[6] > 0)
            {
                if (Regex.IsMatch(match.Value, "\"lat\":(.*?),"))
                {
                    shop.Latitude = Regex.Match(match.Value, "\"lat\":(.*?),").Groups[1].Value;
                }
            }

            if (indexs[7] > 0)
            {
                if (Regex.IsMatch(match.Value, "\"hours\":(.*?),"))
                {
                    string openHoursTemp = Regex.Match(match.Value, "\"hours\":(.*?),").Groups[1].Value.TrimDoubleQuote();
                    string[] openHourMeta = openHoursTemp.Split('|');
                    string openHour = "";
                    foreach(var openHourItem in openHourMeta)
                    {
                        if (openHourItem.Contains("A"))
                        {
                            openHour += Regex.Replace(openHourItem, "A", "Mon: ").Insert(7, ":").Insert(10, " am - ").Insert(18, ":").Insert(21, " pm ");
                        }
                        if (openHourItem.Contains("B"))
                        {
                            openHour += Regex.Replace(openHourItem, "B", "Tue: ").Insert(7, ":").Insert(10, " am - ").Insert(18, ":").Insert(21, " pm ");
                        }
                        if (openHourItem.Contains("C"))
                        {
                            openHour += Regex.Replace(openHourItem, "C", "Wed: ").Insert(7, ":").Insert(10, " am - ").Insert(18, ":").Insert(21, " pm ");
                        }
                        if (openHourItem.Contains("D"))
                        {
                            openHour += Regex.Replace(openHourItem, "D", "Thu: ").Insert(7, ":").Insert(10, " am - ").Insert(18, ":").Insert(21, " pm ");
                        }
                        if (openHourItem.Contains("E"))
                        {
                            openHour += Regex.Replace(openHourItem, "E", "Fri: ").Insert(7, ":").Insert(10, " am - ").Insert(18, ":").Insert(21, " pm ");
                        }
                        if (openHourItem.Contains("F"))
                        {
                            openHour += Regex.Replace(openHourItem, "F", "Sat: ").Insert(7, ":").Insert(10, " am - ").Insert(18, ":").Insert(21, " pm ");
                        }
                        if (openHourItem.Contains("G"))
                        {
                            openHour += Regex.Replace(openHourItem, "G", "Sun: ").Insert(7, ":").Insert(10, " am - ").Insert(18, ":").Insert(21, " pm ");
                        }
                    }
                    shop.OpenHours = openHour;
                }
            }

            if (indexs[8] > 0)
            {
                if (Regex.IsMatch(match.Value, "\"phone\":(.*?),"))
                {
                    shop.Telphone = Regex.Match(match.Value, "\"phone\":(.*?),").Groups[1].Value.TrimDoubleQuote();
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
