using Autofac;
using DentistCalendar.Infrastructure.Commands;
using DentistCalendar.Infrastructure.Commands.Impl;

namespace DentistCalendar.Infrastructure.IoC.Modules
{
    public class CommandModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var ass = ThisAssembly;

            builder.RegisterAssemblyTypes(ass)
                .AsClosedTypesOf(typeof(ICommandHandler<>))
                .InstancePerLifetimeScope();

            builder.RegisterType<CommandDispatcher>()
                .As<ICommandDispatcher>()
                .InstancePerLifetimeScope();
        }
    }
}