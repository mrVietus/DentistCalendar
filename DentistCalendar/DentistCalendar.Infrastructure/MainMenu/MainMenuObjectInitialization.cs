using DentistCalendar.Common.Enums;
using DentistCalendar.Core.Entities;
using System.Collections.Generic;

namespace DentistCalendar.Infrastructure.MainMenu
{
    public class MainMenuObjectInitialization
    {
        public IEnumerable<MenuElement> GetMenuElements()
        {
            var mainMenu = new List<MenuElement>
            {
                new MenuElement
                {
                    MenuElementId = 1,
                    ControllerName = "Admin",
                    AspAction = "Index",
                    DisplayName = "Dashbord",
                    TypeOfPermittedAccount = AccountType.SysAdmin,
                    IsForAuthenticatedUser = true
                },
                new MenuElement
                {
                    MenuElementId = 2,
                    ControllerName = "Dentist",
                    AspAction = "Index",
                    DisplayName = "Dashbord",
                    TypeOfPermittedAccount = AccountType.Dentist,
                    IsForAuthenticatedUser = true
                },
                new MenuElement
                {
                    MenuElementId = 3,
                    ControllerName = "LicenseOwner",
                    AspAction = "Index",
                    DisplayName = "Dashbord",
                    TypeOfPermittedAccount = AccountType.Admin,
                    IsForAuthenticatedUser = true
                },
                new MenuElement
                {
                    MenuElementId = 4,
                    ControllerName = "LicenseOwner",
                    AspAction = "DentistOfficeManagment",
                    DisplayName = "Zarządzanie siecią gabinetów",
                    TypeOfPermittedAccount = AccountType.Admin,
                    IsForAuthenticatedUser = true
                },
                new MenuElement
                {
                    MenuElementId = 5,
                    ControllerName = "Patient",
                    AspAction = "Index",
                    DisplayName = "Dashbord",
                    TypeOfPermittedAccount = AccountType.Patient,
                    IsForAuthenticatedUser = true
                },
                new MenuElement
                {
                    MenuElementId = 6,
                    ControllerName = "Receptionist",
                    AspAction = "Index",
                    DisplayName = "Dashbord",
                    TypeOfPermittedAccount = AccountType.Receptionist,
                    IsForAuthenticatedUser = true
                },
                new MenuElement
                {
                    MenuElementId = 7,
                    ControllerName = "EmployeeManagment",
                    AspAction = "Index",
                    DisplayName = "Zarządzanie pracownikami",
                    TypeOfPermittedAccount = AccountType.Admin,
                    IsForAuthenticatedUser = true
                },
                new MenuElement
                {
                    MenuElementId = 8,
                    ControllerName = "ServiceManagment",
                    AspAction = "Index",
                    DisplayName = "Zarządzanie usługami",
                    TypeOfPermittedAccount = AccountType.Admin,
                    IsForAuthenticatedUser = true
                },
                new MenuElement
                {
                    MenuElementId = 9,
                    ControllerName = "Appointment",
                    AspAction = "MyAppointments",
                    DisplayName = "Moje wizyty",
                    TypeOfPermittedAccount = AccountType.Patient,
                    IsForAuthenticatedUser = true
                }
            };

            return mainMenu;
        }
    }
}
