// <copyright file="HttpClientReader.cs" company="pactera.com">
//     pactera.com. All rights reserved.
// </copyright>

namespace Crawler.HtmlReaders
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Web;
    using Crawler.Helpers;

    public class ValentinoHttpClientReader : IHtmlReader
    {
        protected static readonly Regex CharSetRegex = new Regex("<meta.*charset=(?:\")?(.+?)\"", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        protected HttpClient client = new HttpClient();

        public virtual string GetHtml(string url)
        {
            try
            {
                if (url.Contains("&t=1"))
                {
                    this.client.DefaultRequestHeaders.Add("X-IBM-Client-Id", "88bc9bc2-bef1-42bc-8392-eeef0086db95");
                }
                var response = this.client.GetAsync(url).Result;
                var html = response.Content.ReadAsStringAsync().Result;

                if (response.Content.Headers.ContentType != null && response.Content.Headers.ContentType.CharSet == null && CharSetRegex.IsMatch(html))
                {
                    string charset = CharSetRegex.Match(html).Groups[1].Value;
                    response.Content.Headers.ContentType.CharSet = charset.IndexOf("GB", StringComparison.OrdinalIgnoreCase) > -1 ? "GBK" : charset;
                    html = response.Content.ReadAsStringAsync().Result;
                }

                return HttpUtility.HtmlDecode(html);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public byte[] Download(string url, out string fileName)
        {
            LogHelper.WriteInfo($"Start to download {url}.");
            fileName = string.Empty;
            try
            {
                var response = this.client.GetAsync(url).Result;
                byte[] results = System.Text.Encoding.GetEncoding("utf-8").GetBytes(response.Content.Headers.ToString());
                string value = System.Text.Encoding.GetEncoding("utf-8").GetString(results);
                if (response.StatusCode.GetHashCode() == 200)
                {
                    if (response.Content.Headers.ContentDisposition != null)
                    {
                        fileName = response.Content.Headers.ContentDisposition.FileName.Trim('"');
                    }
                    else
                    {
                        fileName = url.Split('/').Last();
                    }
                    using (Stream htmlStream = response.Content.ReadAsStreamAsync().Result)
                    {
                        MemoryStream memoryStream = new MemoryStream();
                        htmlStream.CopyTo(memoryStream);
                        var result = memoryStream.ToArray();
                        memoryStream.Close();
                        return result;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.WriteError($"Downloading {url} error.", ex);
                return null;
            }
        }
    }
}
