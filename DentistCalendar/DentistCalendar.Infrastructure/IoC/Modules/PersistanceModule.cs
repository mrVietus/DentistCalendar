using Autofac;
using DentistCalendar.Persistence;

namespace DentistCalendar.Infrastructure.IoC.Modules
{
    public class PersistanceModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DentistCalendarDbContext>().AsSelf();
        }
    }
}