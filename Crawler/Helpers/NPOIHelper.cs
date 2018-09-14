using Crawler.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Helpers
{
    public static class NPOIHelper
    {
        public static XSSFWorkbook BuildWorkbook(List<Shop> shops)
        {
            var book = new XSSFWorkbook();
            ISheet sheet = book.CreateSheet(shops.FirstOrDefault()?.MerchantName);
            IRow header = sheet.CreateRow(0);
            ICell sitename = header.CreateCell(0);
            sitename.SetCellValue("商户名");
            ICell url = header.CreateCell(1);
            url.SetCellValue("分店名");
            ICell title = header.CreateCell(2);
            title.SetCellValue("地址");
            ICell category = header.CreateCell(3);
            category.SetCellValue("经度");
            ICell indexCode = header.CreateCell(4);
            indexCode.SetCellValue("纬度");
            ICell issueCode = header.CreateCell(5);
            issueCode.SetCellValue("营业时间");
            ICell publishAgency = header.CreateCell(6);
            publishAgency.SetCellValue("电话");
            ICell publishDate = header.CreateCell(7);
            publishDate.SetCellValue("URL");
            ICell keyword = header.CreateCell(8);
            keyword.SetCellValue("类目");
            ICell shopType = header.CreateCell(9);
            shopType.SetCellValue("门店类型");
            ICell country = header.CreateCell(10);
            country.SetCellValue("国家");
            ICell city = header.CreateCell(11);
            city.SetCellValue("城市");
            ICell scope = header.CreateCell(12);
            scope.SetCellValue("经营范围");
            for (int i = 0; i < shops.Count; i++)
            {
                Shop article = shops[i];
                if (article == null) { continue; }
                IRow row = sheet.CreateRow(i+1);
                ICell cell0 = row.CreateCell(0);
                cell0.SetCellValue(article.MerchantName);
                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue(article.SubbranchName);
                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue(article.Address);
                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue(article.Longitude.ToString());
                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue(article.Latitude.ToString());
                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue(article.OpenHours);
                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue(article.Telphone);
                ICell cell7 = row.CreateCell(7);
                cell7.SetCellValue(article.SiteUrl);
                ICell cell18 = row.CreateCell(8);
                cell18.SetCellValue(article.Category);
                ICell cell19 = row.CreateCell(9);
                cell19.SetCellValue(article.ShopType);
                ICell cell110 = row.CreateCell(10);
                cell110.SetCellValue(article.Country);
                ICell cell111 = row.CreateCell(11);
                cell111.SetCellValue(article.City);
                ICell cell112 = row.CreateCell(12);
                cell112.SetCellValue(article.Scope);
            }
            return book;
        }
    }
}
