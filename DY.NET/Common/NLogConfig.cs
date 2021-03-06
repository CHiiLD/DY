﻿using System.Text;
using NLog;
using NLog.Targets;
using NLog.Config;

namespace DY.NET
{
    /// <summary>
    /// NLog 옵션 설정 클래스
    /// </summary>
    public class NLogConfig
    {
        private static NLogConfig THIS;

        private NLogConfig()
        {
            // Step 1. Create configuration object 
            var config = new LoggingConfiguration();

            // Step 2. Create targets and add them to the configuration 
            var consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);

            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            // Step 3. Set target properties 
            consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";
            fileTarget.FileName = "${basedir}/${date:format=yyyy-MM-dd} log.txt";
            fileTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger}${newline}${message}${newline}";
            fileTarget.Encoding = Encoding.UTF8;

            // Step 4. Define rules
            var rule1 = new LoggingRule("*", LogLevel.Trace, consoleTarget);
            config.LoggingRules.Add(rule1);

            var rule2 = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule2);

            // Step 5. Activate the configuration
            LogManager.Configuration = config;
        }

        /// <summary>
        /// 프로그램 시작과 동시에 호출해야 한다
        /// </summary>
        public static void Load()
        {
            if (THIS == null)
                THIS = new NLogConfig();
        }
    }
}
