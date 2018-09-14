// <copyright file="UrlExtensions.cs" company="pactera.com">
//     pactera.com. All rights reserved.
// </copyright>

namespace Crawler.Helpers
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    public static class HtmlExtension
    {
        public static string TrimContent(this string content)
        {
            string trimLabel = Regex.Replace(content, "<.+?>", string.Empty, RegexOptions.IgnoreCase).Trim();
            string trimSpace = Regex.Replace(trimLabel, @"\s{2,}", " ", RegexOptions.IgnoreCase);
            return trimSpace;
        }

        public static HtmlNode TrimScript(this HtmlNode node)
        {
            var scripts = node?.SelectNodes("//script");
            scripts?.ToList().ForEach(script => script.Remove());
            return node;
        }

        public static string TrimTable(this string content)
        {
            string trimTable = Regex.Replace(content, "\t", string.Empty, RegexOptions.IgnoreCase).Trim();
            return trimTable;
        }

        public static string TrimDoubleQuote(this string content)
        {
            string trimDoubleQuote = Regex.Replace(content, "\"", string.Empty, RegexOptions.IgnoreCase).Trim();
            return trimDoubleQuote;
        }

        public static string TrimLine(this string content)
        {
            string trimLine = Regex.Replace(content, "(\\\\r)?(\\\\n)?", " ", RegexOptions.IgnoreCase).Trim();
            return trimLine;
        }

        public static string TrimEscape(this string content)
        {
            string trimEscape = Regex.Replace(content, "\\\\/", "/", RegexOptions.IgnoreCase).Trim();
            return trimEscape;
        }

        public static string TrimUnicode(this string content)
        {
            if (!Regex.IsMatch(content, @"(?i)\\u([0-9a-f]{4})"))
            {
                return content;
            }
            List<Match> matches = Regex.Matches(content, @"(?i)\\u([0-9a-f]{4})").Cast<Match>().ToList();
            string trimLine = content;
            matches.ForEach(m => {
                trimLine = trimLine.Replace(m.Value.ToString(), ((char)Convert.ToInt32(m.Groups[1].Value, 16)).ToString());
            });
            return trimLine;
        }

        public static string RemoveJsQuote(this string content)
        {
            if (!Regex.IsMatch(content, @""))
            {
                return content;
            }

            string trimJsQuote = Regex.Replace(content, @"/\*[\s\S]*?\*/", "", RegexOptions.IgnoreCase).Trim();
            string trimQuote = Regex.Replace(trimJsQuote, @"//[\s\S]*?;", "", RegexOptions.IgnoreCase).Trim();
            return trimQuote;
        }

        public static string TrimSpace(this string content)
        {
            string trimSpace = Regex.Replace(content, @"\s{1,}", string.Empty, RegexOptions.IgnoreCase);
            return trimSpace;
        }
    }
}
