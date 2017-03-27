using NLog;
using NLog.Config;
using NLog.Targets;

namespace Synthesize.Tests
{
    public static class TestLogger
    {
        public static void Setup()
        {
            var config = new LoggingConfiguration();
            var target = new ColoredConsoleTarget
            {
                Layout = "${level} - ${message}"
            };
            config.AddTarget("console", target);
            var rule = new LoggingRule("*", LogLevel.Debug, target);
            config.LoggingRules.Add(rule);
            LogManager.Configuration = config;
        }
    }
}
