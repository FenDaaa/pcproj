using Crawler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Modes
{
    public class ShopCompare: IEqualityComparer<Shop>
    {
        public bool Equals(Shop x, Shop y)
        {
            if (string.IsNullOrWhiteSpace(x.SiteUrl) && string.IsNullOrWhiteSpace(y.SiteUrl))
            {
                return x.Address.Equals(y.Address, StringComparison.OrdinalIgnoreCase);
            }
            return x.SiteUrl.Equals(y.SiteUrl, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(Shop obj)
        {
            if (string.IsNullOrWhiteSpace(obj.SiteUrl))
            {
                return obj.Address.GetHashCode();
            }
            return obj.SiteUrl.GetHashCode();
        }
    }
}
