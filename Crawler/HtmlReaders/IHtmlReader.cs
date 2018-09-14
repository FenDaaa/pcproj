// <copyright file="IHtmlReader.cs" company="pactera.com">
//     pactera.com. All rights reserved.
// </copyright>

namespace Crawler.HtmlReaders
{
    public interface IHtmlReader
    {
        string GetHtml(string url);

        byte[] Download(string url, out string fileName);
    }
}
