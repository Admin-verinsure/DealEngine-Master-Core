using System;

namespace TechCertain.Domain.Interfaces
{
    public interface ILogger
    {
        bool IsDebugEnabled { get; }
        bool IsErrorEnabled { get; }
        bool IsFatalEnabled { get; }
        bool IsInfoEnabled { get; }
        bool IsWarnEnabled { get; }
        bool IsTraceEnabled { get; }

        void Debug(Exception exception);
        void Debug(string message);
        void Debug(Exception exception, string message);
        void Error(Exception exception);
        void Error(string messages);
        void Error(Exception exception, string message);
        void Fatal(Exception exception);
        void Fatal(string message);
        void Fatal(Exception exception, string message);
        void Info(Exception exception);
        void Info(string message);
        void Info(Exception exception, string message);
        void Trace(Exception exception);
        void Trace(string message);
        void Trace(Exception exception, string message);
        void Warn(Exception exception);
        void Warn(string message);
        void Warn(Exception exception, string message);
    }
}

