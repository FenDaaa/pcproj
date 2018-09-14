// <copyright file="Program.cs" company="pactera.com">
//     pactera.com. All rights reserved.
// </copyright>

namespace Crawler
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using Crawler.Helpers;
    using Crawler.Models;
    using Crawler.SiteCrawler;
    using Newtonsoft.Json;

    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            string log4netPath = AppDomain.CurrentDomain.BaseDirectory + "log4net.xml";
            LogHelper.Initialize(log4netPath);
            string configPath = ConfigurationManager.AppSettings["ConfigurationFile"];
            if (string.IsNullOrWhiteSpace(configPath) || !File.Exists(configPath))
            {
                Console.WriteLine("Configuration file missing. \nPress any key to exit...");
                Console.ReadKey();
                return;
            }

            string config = File.ReadAllText(configPath);
            List<SiteParameter> siteParameters = JsonConvert.DeserializeObject<List<SiteParameter>>(config);

            foreach (var parameter in siteParameters)
            {
                LogHelper.WriteInfo($"Starting crawler for {parameter.MerchantNamePattern}");
                var crawler = CrawlerFactory.Create(parameter);
                crawler.Crawl();
                LogHelper.WriteInfo($"Crawling {parameter.MerchantNamePattern} done.");
            }

#if DEBUG
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
#endif
        }
    }
}
