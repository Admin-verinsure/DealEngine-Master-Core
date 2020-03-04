using System;
using TechCertain.Infrastructure.Logging;
using TechCertain.Domain.Interfaces;
using NLog.Config;

namespace TechCertain.Infrastructure.DependecyResolution
{
    public class LoggingPackage //: IPackage
    {
        #region IPackage implementation

        public void RegisterServices(SimpleInjector.Container container)
        {
            ILogger logger = GetLoggingService();

            container.Register<ILogger>(() => logger);

            //container.Register<ILogger, Log4NetLogger>();           
        }

        #endregion

        private ILogger GetLoggingService()
        {
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("utc_date", typeof(UtcDateRenderer));
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("web_variables", typeof(WebVariablesRenderer));

            ILogger logger = (ILogger)NLog.LogManager.GetLogger("NLog", typeof(LoggingService));

            return logger;
        }
    }
}

