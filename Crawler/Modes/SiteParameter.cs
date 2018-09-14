// <copyright file="SiteParameter.cs" company="pactera.com">
//     pactera.com. All rights reserved.
// </copyright>

namespace Crawler.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public class SiteParameter
    {
        /// <summary>
        /// 品牌
        /// </summary>
        public string MerchantNamePattern { get; set; }

        /// <summary>
        /// 分店
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SubbranchNamePattern { get; set; }

        /// <summary>
        /// 国家
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CountryPattern { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CityPattern { get; set; }

        /// <summary>
        /// 单个匹配规则
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ItemPattern { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AddressPattern { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string LongitudePattern { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string LatitudePattern { get; set; }

        /// <summary>
        /// 开放时间
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string OpenHoursPattern { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string TelphonePattern { get; set; }

        /// <summary>
        /// 门店类型
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ShopTypePattern { get; set; }

        /// <summary>
        /// 类目
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CategoryPattern { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PicturePattern { get; set; }

        /// <summary>
        /// 起始页
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int StartNumber { get; set; }

        /// <summary>
        /// 地址参数
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Tuple<string,string,int>> UrlParams { get; set; }

        /// <summary>
        /// 起始地址
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string StartUrl { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? PageStepNumber { get; set; }

        /// <summary>
        /// 地址规则
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SiteUrlPattern { get; set; }

        /// <summary>
        /// 自定义解析
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> CustomProcessors { get; set; }

        /// <summary>
        /// 解析索引
        /// </summary>
        public string JsonIndexs { get; set; }

        /// <summary>
        /// 解析编码0只解析起始，2解析页面4解析规则
        /// </summary>
        public int HashCode { get; set; }
    }
}
