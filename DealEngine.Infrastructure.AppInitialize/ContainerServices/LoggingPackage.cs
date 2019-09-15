using NLog.Config;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Text;
using TechCertain.Domain.Interfaces;
using TechCertain.Infrastructure.Logging;

namespace DealEngine.Infrastructure.AppInitialize.ContainerServices
{
    public class LoggingPackage
    {
        public static void RegisterServices(Container container)
        {
            ILogger logger = GetLoggingService();

            container.Register<ILogger>(() => logger);

            //container.Register<ILogger, Log4NetLogger>();           
        }

        private static ILogger GetLoggingService()
        {
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("utc_date", typeof(UtcDateRenderer));
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("web_variables", typeof(WebVariablesRenderer));

            ILogger logger = (ILogger)NLog.LogManager.GetLogger("NLog", typeof(LoggingService));

            return logger;
        }
    }
}
