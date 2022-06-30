using Autofac;
using DentistCalendar.Common.Extensions;
using DentistCalendar.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;

namespace DentistCalendar.Infrastructure.IoC.Modules
{
    public class SettingsModule : Autofac.Module
    {
        private readonly IConfiguration Configuration;

        public SettingsModule(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(Configuration.GetSettings<GeneralSettings>())
                .SingleInstance();
            builder.RegisterInstance(Configuration.GetSettings<EmailSettings>())
                .SingleInstance();
        }
    }
}