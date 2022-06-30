using DentistCalendar.Common.Enums;

namespace DentistCalendar.Dto.DTO.Domain
{
    public class MenuElementDto
    {
        public int Id { get; set; }
        public int ParentMenuId { get; set; }
        public AccountType TypeOfPermittedAccount { get; set; }
        public string ControllerName { get; set; }
        public string AspAction { get; set; }
        public string DisplayName { get; set; }
        public bool IsForAuthenticatedUser { get; set; }
    }
}