using DentistCalendar.Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentistCalendar.Core.Entities
{
    public class MenuElement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int MenuElementId { get; set; }
        public int ParentMenuElementId { get; set; }
        public AccountType TypeOfPermittedAccount { get; set; }
        [Required]
        public string ControllerName { get; set; }
        [Required]
        public string AspAction { get; set; }
        [Required]
        public string DisplayName { get; set; }
        [Required]
        public bool IsForAuthenticatedUser { get; set; }

        public bool IsSame(MenuElement otherMenuElement)
        {
            if (otherMenuElement == null) return false;

            if (MenuElementId != otherMenuElement.MenuElementId) return false;
            if (ParentMenuElementId != otherMenuElement.ParentMenuElementId) return false;
            if (ControllerName != otherMenuElement.ControllerName) return false;
            if (AspAction != otherMenuElement.AspAction) return false;
            if (DisplayName != otherMenuElement.DisplayName) return false;
            if (TypeOfPermittedAccount != otherMenuElement.TypeOfPermittedAccount) return false;
            if (IsForAuthenticatedUser != otherMenuElement.IsForAuthenticatedUser) return false;

            return true;
        }

        public void Update(MenuElement menuElement)
        {
            MenuElementId = menuElement.MenuElementId;
            ParentMenuElementId = menuElement.ParentMenuElementId;
            TypeOfPermittedAccount = menuElement.TypeOfPermittedAccount;
            ControllerName = menuElement.ControllerName;
            AspAction = menuElement.AspAction;
            DisplayName = menuElement.DisplayName;
        }
    }
}