using System;

namespace DentistCalendar.Common.Logger
{
    public interface ILoggerService
    {
        #region Public Methods

        void Debug(string message, params object[] args);

        void Debug(string message, Exception exception, params object[] args);

        void Error(string message, params object[] args);

        void Error(string message, Exception exception, params object[] args);

        void Fatal(string message, params object[] args);

        void Fatal(string message, Exception exception, params object[] args);

        void Info(string message, params object[] args);

        void Info(string message, Exception exception, params object[] args);

        void Trace(string message, params object[] args);

        void Trace(string message, Exception exception, params object[] args);

        void Warn(string message, params object[] args);

        void Warn(string message, Exception exception, params object[] args);

        #endregion
    }
}
