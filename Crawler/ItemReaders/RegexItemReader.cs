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

    public class RegexItemReader : IItemReader
    {
        private SiteParameter siteParameter;
        private Regex pattern;

        public RegexItemReader(SiteParameter siteParameter)
        {
            this.siteParameter = siteParameter ?? throw new ArgumentNullException(nameof(siteParameter));
            this.pattern = new Regex(siteParameter.ItemPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        }

        public IEnumerable<Shop> GetShops(string html, string pageUrl)
        {
            List<Match> matches = this.pattern.Matches(html).Cast<Match>().ToList();
            List<Match> list = new List<Match>();
            if (this.siteParameter.MerchantNamePattern.Equals("家乐福") && this.siteParameter.CountryPattern.Equals("Indonesia"))
            {
                list = Regex.Matches(html, "new google.maps.LatLng\\((.+?), (.+?)\\)").Cast<Match>().ToList();
            }
            IEnumerable<Shop> shops = matches
                .Select(match =>
                {
                    {
                        Shop shop = FormatShop(match, pageUrl);
                        if (this.siteParameter.MerchantNamePattern.Equals("家乐福") && this.siteParameter.CountryPattern.Equals("Indonesia"))
                        {
                            Match latLng = list[matches.IndexOf(match)];
                            shop.Latitude = latLng.Groups[1].Value;
                            shop.Longitude = latLng.Groups[2].Value;
                        }
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
                if (this.siteParameter.MerchantNamePattern.Equals("麦当劳") && this.siteParameter.CountryPattern.Equals("USA"))
                {
                    shop.Address = (match.Groups[3].Value + match.Groups[4].Value + "," + match.Groups[6].Value).TrimUnicode();
                }
                else
                {
                    shop.Address = match.Groups[indexs[4]].Value.TrimContent().TrimDoubleQuote().TrimLine().TrimEscape();
                }
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
                if (this.siteParameter.MerchantNamePattern.Equals("屈臣氏") && this.siteParameter.CityPattern.Equals("HongKong&Macao"))
                {
                    List<string> list = Regex.Matches(match.Groups[indexs[7]].Value, "<span class=\"store-address-info\">(.+?)</span>").Cast<Match>().Select(s => s.Groups[1].Value).ToList();
                    shop.OpenHours = string.Join(",", list);
                }
                else if (this.siteParameter.MerchantNamePattern.Equals("麦当劳") && this.siteParameter.CityPattern.Equals("HongKong&Macao"))
                {
                    List<string> list = Regex.Matches(match.Groups[indexs[7]].Value, "{\"openTime\":(.+?,\"closeTime\":.+?)}").Cast<Match>().Select(s => s.Groups[1].Value).ToList();
                    if (list.Distinct().Count() == 1)
                    {
                        list = list.Distinct().ToList().Select(s => s.TrimDoubleQuote().Replace(",", "-").Replace("closeTime:", string.Empty)).ToList();
                    }
                    else
                    {
                        list = list.Select(s => s.TrimDoubleQuote().Replace(",", "-").Replace("closeTime:", string.Empty)).ToList();
                    }
                    shop.OpenHours = string.Join(",", list);
                }
                else if (this.siteParameter.MerchantNamePattern.Equals("肯德基") && this.siteParameter.CityPattern.Equals("HongKong&Macao"))
                {
                    shop.OpenHours = match.Groups[indexs[7]].Value.TrimContent().TrimDoubleQuote().TrimLine().TrimUnicode();
                    if (!string.IsNullOrWhiteSpace(match.Groups[4].Value))
                    {
                        shop.OpenHours = shop.OpenHours + "," + match.Groups[4].Value;
                    }
                }
                else if (this.siteParameter.MerchantNamePattern.Equals("麦当劳") && this.siteParameter.CountryPattern.Equals("USA"))
                {
                    shop.OpenHours = match.Groups[indexs[7]].Value.TrimContent().TrimDoubleQuote().Replace("hours", string.Empty);
                }
                else if (this.siteParameter.MerchantNamePattern.Equals("华伦天奴"))
                {
                    List<Match> matches = Regex.Matches(match.Groups[indexs[7]].Value, "{\"dayNumber\":(\\d),.*?\"openTime\":\"(.*?)\",\"closeTime\":\"(.*?)\"}").Cast<Match>().ToList();
                    Dictionary<string, string> dic = new Dictionary<string, string>() { { "1", "Monday" }, { "2", "Tuesday" }, { "3", "Wednesday" }, { "4", "Thursday" }, { "5", "Friday" }, { "6", "Saturday" }, { "7", "Sunday" } };
                    StringBuilder sb = new StringBuilder();
                    matches.ForEach(m =>
                    {
                        sb.Append(string.Format("{0}:{1}-{2}", dic[m.Groups[1].Value], m.Groups[2].Value, m.Groups[3].Value));
                    });
                    shop.OpenHours = sb.ToString().Trim(',');
                }
                else if (this.siteParameter.MerchantNamePattern.Equals("屈臣氏") && this.siteParameter.CityPattern.Equals("TaiWan"))
                {
                    if (Regex.IsMatch(match.Value, "營業時間\\s+?([\\s\\S]*?)<\\/span>"))
                    {
                        shop.OpenHours = Regex.Match(match.Value, "營業時間\\s+?([\\s\\S]*?)<\\/span>").Groups[1].Value.TrimContent();
                    }
                }
                else
                {
                    shop.OpenHours = match.Groups[indexs[7]].Value.TrimContent().TrimDoubleQuote().TrimLine().TrimUnicode();
                }
            }

            if (indexs[8] > 0)
            {
                if (this.siteParameter.MerchantNamePattern.Equals("家乐福") && this.siteParameter.CountryPattern.Equals("Indonesia"))
                {
                    string value = Regex.Replace(match.Groups[indexs[8]].Value, "Fax.+?<\\/p>", string.Empty).TrimContent().Replace("Operator -", string.Empty).Replace("Operator", string.Empty).Replace("Hotline -", string.Empty).Replace("Tel.", string.Empty).Replace("Hotline", string.Empty).Replace("Hotline (Mobile)", string.Empty).Replace("(Mobile) - ", string.Empty).Replace("(Mobile)", string.Empty);
                    shop.Telphone = value;
                }
                else if (this.siteParameter.MerchantNamePattern.Equals("屈臣氏") && this.siteParameter.CityPattern.Equals("TaiWan"))
                {
                    if (Regex.IsMatch(match.Value, "<a href=\"tel:.*?>(.*?)<\\/a>"))
                    {
                        shop.Telphone = Regex.Match(match.Value, "<a href=\"tel:.*?>(.*?)<\\/a>").Groups[1].Value.TrimContent();
                    }
                }
                else
                {
                    shop.Telphone = match.Groups[indexs[8]].Value.Trim().TrimContent().TrimLine();
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
