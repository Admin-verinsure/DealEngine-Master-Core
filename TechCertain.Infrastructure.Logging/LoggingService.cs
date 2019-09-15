using System;
using NLog;

namespace TechCertain.Infrastructure.Logging
{
	public class LoggingService : Logger, Domain.Interfaces.ILogger
    {
		private const string _LoggerName = "NLog";

		#region ILoggingService implementation

		public void Debug (Exception exception)
		{
			this.Debug (exception, string.Empty);
		}

		public new void Debug (Exception exception, string message)
		{
			if (!base.IsDebugEnabled)
				return;

			var logEvent = GetLogEvent (_LoggerName, LogLevel.Debug, exception, message);
			base.Log (typeof(LoggingService), logEvent);
		}

		public void Error (Exception exception)
		{
			this.Error (exception, string.Empty);
		}

        public new void Error(Exception exception, string message)
		{
			if (!base.IsErrorEnabled)
				return;            

			var logEvent = GetLogEvent (_LoggerName, LogLevel.Error, exception, message);
			base.Log (typeof(LoggingService), logEvent);
		}

		public void Fatal (Exception exception)
		{
			this.Fatal (exception, string.Empty);
		}

		public new void Fatal (Exception exception, string message)
		{
			if (!base.IsFatalEnabled)
				return;

			var logEvent = GetLogEvent (_LoggerName, LogLevel.Fatal, exception, message);
			base.Log (typeof(LoggingService), logEvent);
		}

		public void Info (Exception exception)
		{
			this.Info (exception, string.Empty);
		}

		public new void Info (Exception exception, string message)

        {
			if (!base.IsInfoEnabled)
				return;

			var logEvent = GetLogEvent (_LoggerName, LogLevel.Info, exception, message);
			base.Log (typeof(LoggingService), logEvent);
		}

		public void Trace (Exception exception)
		{
			this.Trace (exception, string.Empty);
		}

		public new void Trace (Exception exception, string message)
		{
			if (!base.IsTraceEnabled)
				return;

			var logEvent = GetLogEvent (_LoggerName, LogLevel.Trace, exception, message);
			base.Log (typeof(LoggingService), logEvent);
		}

		public void Warn (Exception exception)
		{
			this.Warn (exception, string.Empty);
		}

		public new void Warn (Exception exception, string message)
		{
			if (!base.IsWarnEnabled)
				return;

			var logEvent = GetLogEvent (_LoggerName, LogLevel.Warn, exception, message);
			base.Log (typeof(LoggingService), logEvent);
		}

		#endregion

		private LogEventInfo GetLogEvent (string loggerName, LogLevel level, Exception exception, string message)
		{
			string assemblyProp = string.Empty;
			string classProp = string.Empty;
			string methodProp = string.Empty;
			string messageProp = string.Empty;
			string innerMessageProp = string.Empty;

			var logEvent = new LogEventInfo (level, loggerName, message);

			if (exception != null) {
				assemblyProp = exception.Source;
				classProp = exception.TargetSite.DeclaringType.FullName;
				methodProp = exception.TargetSite.Name;
				messageProp = exception.Message;
				logEvent.Exception = exception;

				if (exception.InnerException != null) {
					innerMessageProp = exception.InnerException.Message;
				}
			}

			logEvent.Properties ["error-source"] = assemblyProp;
			logEvent.Properties ["error-class"] = classProp;
			logEvent.Properties ["error-method"] = methodProp;
			logEvent.Properties ["error-message"] = messageProp;
			logEvent.Properties ["inner-error-message"] = innerMessageProp;

			return logEvent;
		}
	}
    
}

