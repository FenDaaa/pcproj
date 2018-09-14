// <copyright file="IPageReader.cs" company="pactera.com">
//     pactera.com. All rights reserved.
// </copyright>

namespace Crawler.PageReaders
{
    using System.Collections.Generic;
    using Crawler.Models;

    public interface IPageReader
    {
        IEnumerable<Shop> GetShops();
    }
}
