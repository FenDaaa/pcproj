// <copyright file="IItemReader.cs" company="pactera.com">
//     pactera.com. All rights reserved.
// </copyright>

namespace Crawler.ItemReaders
{
    using System.Collections.Generic;
    using Crawler.Models;

    public interface IItemReader
    {
        IEnumerable<Shop> GetShops(string html, string pageUrl);
    }
}