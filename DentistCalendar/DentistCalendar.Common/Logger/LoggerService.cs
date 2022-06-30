using System;

namespace DentistCalendar.Common.Logger
{
    public class LoggerService : ILoggerService
    {
        private NLog.Logger _logger;

        public LoggerService(NLog.Logger logger)
        {
            _logger = logger;
        }

        #region Public Methods

        public void Debug(string message, params object[] args)
        {
            _logger.Debug(message, args);
        }

        public void Debug(string message, Exception exception, params object[] args)
        {
            _logger.Debug(message, exception, args);
        }

        public void Error(string message, params object[] args)
        {
            _logger.Error(message, args);
        }

        public void Error(string message, Exception exception, params object[] args)
        {
            _logger.Error(message, exception, args);
        }

        public void Fatal(string message, params object[] args)
        {
            _logger.Fatal(message, args);
        }

        public void Fatal(string message, Exception exception, params object[] args)
        {
            _logger.Fatal(message, exception, args);
        }

        public void Info(string message, params object[] args)
        {
            _logger.Info(message, args);
        }

        public void Info(string message, Exception exception, params object[] args)
        {
            _logger.Info(message, exception, args);
        }

        public void Trace(string message, params object[] args)
        {
            _logger.Trace(message, args);
        }

        public void Trace(string message, Exception exception, params object[] args)
        {
            _logger.Trace(message, exception, args);
        }

        public void Warn(string message, params object[] args)
        {
            _logger.Warn(message, args);
        }

        public void Warn(string message, Exception exception, params object[] args)
        {
            _logger.Warn(message, exception, args);
        }

        #endregion
    }
}
