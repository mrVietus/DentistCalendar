namespace DentistCalendar.Infrastructure.Commands.Impl.Email
{
    public class SendConfirmRegistrationEmail : ICommand
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}