// <copyright file="CrawlerFactory.cs" company="pactera.com">
//     pactera.com. All rights reserved.
// </copyright>

namespace Crawler.SiteCrawler
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using Crawler.HtmlReaders;
    using Crawler.ItemReaders;
    using Crawler.PageParsers;
    using Crawler.PageReaders;
    using Crawler.Models;
    using Microsoft.Practices.Unity.Configuration;
    using Unity;
    using Unity.Resolution;

    public static class CrawlerFactory
    {
        private static readonly IUnityContainer Container = new UnityContainer();

        static CrawlerFactory()
        {
            UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            section.Configure(Container, "Crawler");
        }

        public static ISiteCrawler Create(SiteParameter siteParameter)
        {
            if (siteParameter == null)
            {
                throw new ArgumentNullException(nameof(siteParameter));
            }

            string dataServiceName = GetValueOrDefault(siteParameter.CustomProcessors, "IDataService");
            string htmlReaderName = GetValueOrDefault(siteParameter.CustomProcessors, "IHtmlReader");
            string pageParserName = GetValueOrDefault(siteParameter.CustomProcessors, "IPageParser");
            string itemReaderName = GetValueOrDefault(siteParameter.CustomProcessors, "IItemReader");
            string pageReaderName = GetValueOrDefault(siteParameter.CustomProcessors, "IPageReader");

            IHtmlReader htmlReader = Container.Resolve<IHtmlReader>(htmlReaderName);
            ParameterOverride htmlReaderParameter = new ParameterOverride("htmlReader", htmlReader);

            ParameterOverride siteParameterParameter = new ParameterOverride("siteParameter", siteParameter);

            IItemReader itemReader = Container.Resolve<IItemReader>(itemReaderName, siteParameterParameter);
            ParameterOverride itemReaderParameter = new ParameterOverride("itemReader", itemReader);

            IPageReader pageReader = Container.Resolve<IPageReader>(pageReaderName, siteParameterParameter, htmlReaderParameter, itemReaderParameter);

            IPageParser pageParser = Container.Resolve<IPageParser>(pageParserName, siteParameterParameter, htmlReaderParameter);

            return new GeneralSiteCrawler(pageReader, pageParser);
        }

        private static string GetValueOrDefault(Dictionary<string, string> dictionary, string key)
        {
            string defaultValue = "Default";
            if (dictionary == null)
            {
                return defaultValue;
            }

            if (!dictionary.ContainsKey(key))
            {
                return defaultValue;
            }

            return dictionary[key];
        }
    }
}
