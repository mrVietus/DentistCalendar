using Autofac;
using DentistCalendar.Common.Logger;
using NLog;

namespace DentistCalendar.Infrastructure.IoC.Modules
{
    public class LoggerModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => LogManager.GetLogger("AppLogger", typeof(NLog.Logger))).SingleInstance();

            builder.RegisterType<LoggerService>()
               .As<ILoggerService>()
               .InstancePerLifetimeScope();
        }
    }
}