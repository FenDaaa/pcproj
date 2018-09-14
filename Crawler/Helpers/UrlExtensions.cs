// <copyright file="UrlExtensions.cs" company="pactera.com">
//     pactera.com. All rights reserved.
// </copyright>

namespace Crawler.Helpers
{
    using System;
    using System.Web;

    public static class UrlExtensions
    {
        public static string ToAbsoluteUrl(this string url, string baseUrl)
        {
            Uri baseUri = new Uri(baseUrl);
            Uri absoluteUri = new Uri(baseUri, url);
            return HttpUtility.UrlDecode(absoluteUri.AbsoluteUri);
        }

        public static string UrlDecode(this string url)
        {
            return HttpUtility.UrlDecode(url);
        }
    }
}
