using DentistCalendar.Common.Enums;

namespace DentistCalendar.Infrastructure.Commands.Impl.Users
{

    //Not used Just stayd here if in some time it will be some need to use command handler pattern

    public class CreateUser : ICommand
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public AccountType AccountType { get; set; }
    }
}