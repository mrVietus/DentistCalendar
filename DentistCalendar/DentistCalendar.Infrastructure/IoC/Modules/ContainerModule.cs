using Autofac;
using DentistCalendar.Infrastructure.Mappers;
using Microsoft.Extensions.Configuration;

namespace DentistCalendar.Infrastructure.IoC.Modules
{
    public class ContainerModule : Autofac.Module
    {
        private readonly IConfiguration Configuration;

        public ContainerModule(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(AutoMapperConfig.Initialize())
                .SingleInstance();

            builder.RegisterModule<CommandModule>();
            builder.RegisterModule<LoggerModule>();
            builder.RegisterModule<PersistanceModule>();
            builder.RegisterModule<RepositoryModule>();
            builder.RegisterModule<ServicesModule>();
            builder.RegisterModule(new SettingsModule(Configuration));
        }
    }
}