namespace DentistCalendar.Infrastructure.MappedModels.MainMenu
{
    public class MainMenuModel
    {
        public string ControllerName { get; set; }
        public string AspAction { get; set; }
        public string DisplayName { get; set; }
        public bool IsForAuthenticatedUser { get; set; }
    }
}