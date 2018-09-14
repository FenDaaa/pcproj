// <copyright file="RegexPageParser.cs" company="pactera.com">
//     pactera.com. All rights reserved.
// </copyright>

namespace Crawler.PageParsers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Crawler.Helpers;
    using Crawler.HtmlReaders;
    using Crawler.Models;
    using HtmlAgilityPack;

    public class RegexPageParser : IPageParser
    {
        public RegexPageParser(SiteParameter siteParameter, IHtmlReader htmlReader)
        {
            this.SiteParameter = siteParameter ?? throw new ArgumentNullException(nameof(siteParameter));
            this.HtmlReader = htmlReader ?? throw new ArgumentNullException(nameof(htmlReader));
        }

        protected SiteParameter SiteParameter { get; }

        protected IHtmlReader HtmlReader { get; }

        public virtual Shop GetShopDetails(Shop shop)
        {
            try
            {
                string content = this.HtmlReader.GetHtml(shop.SiteUrl);
                if (string.IsNullOrWhiteSpace(content))
                {
                    LogHelper.WriteInfo($"{shop.SiteUrl} is not reachable.");
                    return shop;
                }

                LogHelper.WriteInfo($"Getting details {shop.SiteUrl}");
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(content);
                shop.MerchantName = this.SiteParameter.MerchantNamePattern;
                List<int> indexs = this.SiteParameter.JsonIndexs.Split(',').Select(s => int.Parse(s)).ToList();
                if (!string.IsNullOrWhiteSpace(this.SiteParameter.SubbranchNamePattern) && indexs[1] < 0)
                {
                    shop.SubbranchName = MatchedValue(this.SiteParameter.SubbranchNamePattern, document);
                }

                if (!string.IsNullOrWhiteSpace(this.SiteParameter.AddressPattern) && indexs[4] < 0)
                {
                    shop.Address = MatchedValue(this.SiteParameter.AddressPattern, document);
                }

                if (!string.IsNullOrWhiteSpace(this.SiteParameter.LongitudePattern) && indexs[5] < 0)
                {
                    shop.Longitude = MatchedValue(this.SiteParameter.LongitudePattern, document);
                }

                if (!string.IsNullOrWhiteSpace(this.SiteParameter.LatitudePattern) && indexs[6] < 0)
                {
                    shop.Latitude = MatchedValue(this.SiteParameter.LatitudePattern, document);
                }

                if (!string.IsNullOrWhiteSpace(this.SiteParameter.OpenHoursPattern) && indexs[7] < 0)
                {
                    shop.OpenHours = MatchedValue(this.SiteParameter.OpenHoursPattern, document);
                }

                if (!string.IsNullOrWhiteSpace(this.SiteParameter.TelphonePattern) && indexs[8] < 0)
                {
                    shop.Telphone = MatchedValue(this.SiteParameter.TelphonePattern, document);
                }

                if (!string.IsNullOrWhiteSpace(this.SiteParameter.ShopTypePattern) && indexs[9] < 0)
                {
                    shop.ShopType = MatchedValue(this.SiteParameter.ShopTypePattern, document);
                }

                shop.Category = this.SiteParameter.CategoryPattern;
                if (!string.IsNullOrWhiteSpace(this.SiteParameter.PicturePattern) && indexs[11] < 0)
                {
                    shop.Picture = MatchedValue(this.SiteParameter.PicturePattern, document);
                }

                if (string.IsNullOrWhiteSpace(shop.Country))
                {
                    shop.Country = this.SiteParameter.CountryPattern;
                }

                if (string.IsNullOrWhiteSpace(shop.City))
                {
                    shop.City = this.SiteParameter.CityPattern;
                }
                return shop;
            }
            catch (Exception ex)
            {
                LogHelper.WriteError($"Fetching {shop.SiteUrl} error.", ex);
                return null;
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
