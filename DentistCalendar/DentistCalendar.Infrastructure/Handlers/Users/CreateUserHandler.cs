using DentistCalendar.Infrastructure.Commands;
using DentistCalendar.Infrastructure.Commands.Impl.Users;
using DentistCalendar.Infrastructure.Services;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Handlers.Users
{
    //Not used Just stayd here if in some time it will be some need to use command handler pattern

    public class CreateUserHandler : ICommandHandler<CreateUser>
    {
        private readonly IUserService UserService;

        public CreateUserHandler(IUserService userService)
        {
            UserService = userService;
        }

        public async Task HandleAsync(CreateUser command)
        {
            //await UserService.TryRegisterAsync(command.Email, command.Password, command.AccountType);
        }
    }
}