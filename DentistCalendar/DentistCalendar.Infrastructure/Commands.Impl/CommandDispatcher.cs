using Autofac;
using System;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Commands.Impl
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IComponentContext Context;

        public CommandDispatcher(IComponentContext context)
        {
            Context = context;
        }

        public async Task DispatchAsync<T>(T command) where T : ICommand
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command),
                    $"Command {typeof(T).Name} can't be null");
            }

            var handler = Context.Resolve<ICommandHandler<T>>();

            await handler.HandleAsync(command);
        }
    }
}