using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Models
{
    public class Shop
    {
        public Shop()
        {
            MerchantName = string.Empty;
            SubbranchName = string.Empty;
            Address = string.Empty;
            Longitude = string.Empty;
            Latitude = string.Empty;
            OpenHours = string.Empty;
            Telphone = string.Empty;
            ShopType = string.Empty;
            Category = string.Empty;
            Picture = string.Empty;
            SiteUrl = string.Empty;
        }
        public string MerchantName { get; set; }

        public string SubbranchName { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string Address { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public string Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public string Latitude { get; set; }

        public string OpenHours { get; set; }

        public string Telphone { get; set; }

        /// <summary>
        /// 门店类型
        /// </summary>
        public string ShopType { get; set; }

        /// <summary>
        /// 类目
        /// </summary>
        public string Category { get; set; }

        public string Picture { get; set; }

        public string SiteUrl { get; set; }

        /// <summary>
        /// 经营范围
        /// </summary>
        public string Scope { get; set; }
    }
}
