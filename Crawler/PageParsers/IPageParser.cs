// <copyright file="IPageParser.cs" company="pactera.com">
//     pactera.com. All rights reserved.
// </copyright>

namespace Crawler.PageParsers
{
    using System;
    using System.Collections.Generic;
    using Crawler.Models;

    public interface IPageParser
    {
        Shop GetShopDetails(Shop shop);
    }
}
