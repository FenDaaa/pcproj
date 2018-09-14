// <copyright file="LogHelper.cs" company="zhyu2014@163.com">
//     pactera.com. All rights reserved.
// </copyright>

namespace Crawler.Helpers
{
    using System;
    using System.IO;
    using log4net;
    using log4net.Config;
    using log4net.Repository;

    public static class LogHelper
    {
        private static ILog log = null;
        /// <summary>
        /// 初始化日志类
        /// </summary>
        public static void Initialize(string configPath)
        {
            ILoggerRepository repository = LogManager.CreateRepository("logRepository");
            try
            {
                XmlConfigurator.Configure(repository, new FileInfo(configPath));
                log = LogManager.GetLogger(repository.Name, "logInstance");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 写正常日志
        /// </summary>
        /// <param name="content">日志内容</param>
        public static void WriteInfo(string content)
        {
            log.Info(content);
        }

        /// <summary>
        /// 写错误日志
        /// </summary>
        /// <param name="content">日志</param>
        /// <param name="ex">日志内容</param>
        public static void WriteError(string content, Exception ex)
        {
            log.Error(content, ex);
        }
    }
}
