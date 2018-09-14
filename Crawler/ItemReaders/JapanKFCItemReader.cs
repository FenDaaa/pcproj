using Crawler.Helpers;
using Crawler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Crawler.ItemReaders
{
    public class JapanKFCItemReader: IItemReader
    {
        private SiteParameter siteParameter;
        private Regex pattern;

        public JapanKFCItemReader(SiteParameter siteParameter)
        {
            this.siteParameter = siteParameter ?? throw new ArgumentNullException(nameof(siteParameter));
            this.pattern = new Regex(siteParameter.ItemPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        }

        public IEnumerable<Shop> GetShops(string html, string pageUrl)
        {
            MatchCollection matches = this.pattern.Matches(html);
            IEnumerable<Shop> shops = matches
                .Cast<Match>()
                .Select(match =>
                {
                    return FormatShop(match, pageUrl);
                });

            return shops;
        }

        public Shop FormatShop(Match match, string pageUrl)
        {
            Shop shop = new Shop();
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
                shop.City = match.Groups[indexs[3]].Value.TrimUnicode();
            }

            if (indexs[4] > 0)
            {
                shop.Address = match.Groups[indexs[4]].Value.TrimContent().TrimDoubleQuote().TrimLine().TrimEscape().TrimUnicode();
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
                if (this.siteParameter.MerchantNamePattern.Equals("肯德基") && this.siteParameter.CountryPattern.Equals("Malaysia"))
                {
                    List<Group> groups = match.Groups.Cast<Group>().Skip(6).Take(5).ToList();
                    if (groups[0].Value.Equals("true"))
                    {
                        shop.OpenHours = "Open 24 hours";
                    }
                    else
                    {
                        shop.OpenHours = string.Format("Monday-Friday {0}-{1},Saturday-Sunday:{2}-{3}", groups[1].Value, groups[2].Value, groups[3].Value, groups[4].Value);
                    }
                }
                else
                {
                    shop.OpenHours = match.Groups[indexs[7]].Value.TrimContent().TrimDoubleQuote().TrimLine().TrimUnicode();
                }
            }

            if (indexs[8] > 0)
            {
                shop.Telphone = match.Groups[indexs[8]].Value.Trim().TrimContent().TrimLine();
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
                if (this.siteParameter.MerchantNamePattern.Equals("肯德基") && this.siteParameter.CountryPattern.Equals("Malaysia"))
                {
                    shop.SiteUrl = string.Format("https://kfc.com.my/find-a-kfc/{0}", match.Groups[indexs[12]].Value);
                }
                else if (this.siteParameter.MerchantNamePattern.Equals("家乐福") && this.siteParameter.CityPattern.Equals("TaiWan"))
                {
                    shop.SiteUrl = string.Format("https://www.carrefour.com.tw/storedetail.html?storeid={0}", match.Groups[indexs[12]].Value);
                }
                else
                {
                    shop.SiteUrl = string.Format("https://www.kfc.co.jp/search/detail/?shop_id={0}", match.Groups[indexs[12]].Value);
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
