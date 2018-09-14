using Crawler.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Crawler.HtmlReaders
{
    public class HttpClientUserAgentReader : IHtmlReader
    {
        protected static readonly Regex CharSetRegex = new Regex("<meta.*charset=(?:\")?(.+?)\"", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        protected HttpClient client = new HttpClient();
        private static Random ran = new Random();

        public virtual string GetHtml(string url)
        {
            try
            {
                this.client.DefaultRequestHeaders.Add("User-Agent", RandomAgent(ran.Next(1,10)));
                var response = this.client.GetAsync(url).Result;
                if (response.Content.Headers.ContentType != null && response.Content.Headers.ContentType.MediaType != "text/html" && response.Content.Headers.ContentType.MediaType != "application/json" && response.Content.Headers.ContentType.MediaType != "text/plain")
                {
                    return "[Not a html page.]";
                }

                var html = response.Content.ReadAsStringAsync().Result;

                if (response.Content.Headers.ContentType != null && response.Content.Headers.ContentType.CharSet == null && CharSetRegex.IsMatch(html))
                {
                    string charset = CharSetRegex.Match(html).Groups[1].Value;
                    response.Content.Headers.ContentType.CharSet = charset.IndexOf("GB", StringComparison.OrdinalIgnoreCase) > -1 ? "GBK" : charset;
                    html = response.Content.ReadAsStringAsync().Result;
                }

                return HttpUtility.HtmlDecode(html);
            }
            catch (Exception ex)
            {
                this.client = new HttpClient();
                throw ex;            
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

        public string RandomAgent(int random)
        {
            switch(random)
            {
                case 1:
                    return "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
                case 2:
                    return "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:6.0) Gecko/20100101 Firefox/6.0";
                case 3:
                    return "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/534.50 (KHTML, like Gecko) Version/5.1 Safari/534.50";
                case 4:
                    return "Opera/9.80 (Windows NT 6.1; U; zh-cn) Presto/2.9.168 Version/11.50";
                case 5:
                    return "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Win64; x64; Trident/5.0; .NET CLR 2.0.50727; SLCC2; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.3; .NET4.0C; Tablet PC 2.0; .NET4.0E)";
                case 6:
                    return "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; InfoPath.3)";
                case 7:
                    return "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.3; .NET4.0C; .NET4.0E)";
                case 8:
                    return "Mozilla/5.0 (Windows; U; Windows NT 6.1; ) AppleWebKit/534.12 (KHTML, like Gecko) Maxthon/3.0 Safari/534.12";
                case 9:
                    return "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.3 (KHTML, like Gecko) Chrome/6.0.472.33 Safari/534.3 SE 2.X MetaSr 1.0";
                case 10:
                    return "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
                default:
                    return "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
            }
        }
    }
}
