// <copyright file="ISiteCrawler.cs" company="pactera.com">
//     pactera.com. All rights reserved.
// </copyright>

using Crawler.Models;
using System.Collections.Generic;

namespace Crawler.SiteCrawler
{
    public interface ISiteCrawler
    {
        void Crawl();

        void WriteExcel(IEnumerable<Shop> shops);
    }
}
