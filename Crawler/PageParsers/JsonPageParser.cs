using Crawler.HtmlReaders;
using Crawler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.PageParsers
{
    public class JsonPageParser: IPageParser
    {
        public JsonPageParser(SiteParameter siteParameter, IHtmlReader htmlReader)
        {
            this.SiteParameter = siteParameter ?? throw new ArgumentNullException(nameof(siteParameter));
            this.HtmlReader = htmlReader ?? throw new ArgumentNullException(nameof(htmlReader));
        }

        protected SiteParameter SiteParameter { get; }

        protected IHtmlReader HtmlReader { get; }

        public virtual Shop GetShopDetails(Shop shop)
        {
            shop.MerchantName = this.SiteParameter.MerchantNamePattern;
            shop.Category = this.SiteParameter.CategoryPattern;
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
    }
}
